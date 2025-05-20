using Neuro_SDK_Csharp.Messages.Outgoing;
using Neuro_SDK_Csharp.Utilities;
using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Actions;

public sealed class ActionWindow
{
    #region Create
    private ActionWindow()
    {}
    
    public static ActionWindow Create()
    {
        return new ActionWindow();
    }
    #endregion
    
    #region State

    

    public enum State
    {
        Building,
        Registered,
        Forced,
        Ended,
    }

    public State CurrentState { get; private set; } = State.Building;

    private bool ValidateFrozen()
    {
        if (CurrentState != State.Building)
        {
            Console.WriteLine("Cannot change action window after it has been registered");
            return false;
        }

        return true;
    }

    public void Register()
    {
        if (CurrentState != State.Building)
        {
            Console.WriteLine("Cannot register action window more than once");
            return;
        }

        if (_actions.Count == 0)
        {
            Console.WriteLine("Cannot register action window with not actions");
            return;
        }

        if (_contextMessage is not (null or ""))
        {
            Context.Send(_contextMessage,_contextSilent!.Value);
        }
        NeuroActionHandler.RegisterActions(_actions);

        CurrentState = State.Registered;
    }
    #endregion

    #region Context

    

    private string? _contextMessage;
    private bool? _contextSilent;

    public ActionWindow SetContext(string message, bool silent = false)
    {
        if (!ValidateFrozen()) return this;

        _contextMessage = message;
        _contextSilent = silent;

        return this;
    }
    #endregion

    #region Actions
    
    private readonly List<INeuroAction> _actions = new();

    public ActionWindow AddAction(INeuroAction action)
    {
        if (!ValidateFrozen()) return this;

        if (action.ActionWindow != null)
        {
            if (action.ActionWindow != this)
            {
                Console.WriteLine($"Cannot add action {action.Name} to this action window");
            }

            return this;
        }

        if (!action.CanAddToActionWindow(this)) return this;

        if (_actions.Any(a => a.Name == action.Name))
        {
            Console.WriteLine($"Cannot add two action that have the same name. The name was {action.Name}");
            return this;
        }
        
        action.SetActionWindow(this);
        _actions.Add(action);

        return this;
    }
    
    #endregion

    #region Force
    
    private Func<bool>? _shouldForceFunc;
    private Func<string>? _forceQueryGetter;
    private Func<string?>? _forceStateGetter;
    private bool? _forceEphemeralContext;

    public ActionWindow SetForce(Func<bool> shouldForce, Func<string> queryGetter, Func<string?> stateGetter,
        bool ephemeralContext = false)
    {
        if (!ValidateFrozen()) return this;

        _shouldForceFunc = shouldForce;
        _forceQueryGetter = queryGetter;
        _forceStateGetter = stateGetter;
        _forceEphemeralContext = ephemeralContext;

        return this;
    }

    public ActionWindow SetForce(Func<bool> shouldForce, string query, string? state, bool ephemeralContext = false) =>
        SetForce(shouldForce, () => query, () => state, ephemeralContext);

    public ActionWindow SetForce(float afterSeconds, Func<string> queryGetter, Func<string?> stateGetter,
        bool ephemeralContext = false)
    {
        float time = afterSeconds;
        
        return SetForce(ShouldForce, queryGetter, stateGetter, ephemeralContext);

        bool ShouldForce()
        {
            time -= Time.GetDelta(); 
            return time <= 0;
        }
    }

    public ActionWindow SetForce(float afterSeconds, string query, string? state, bool ephemeralContext = false) =>
        SetForce(afterSeconds, () => query,() => state, ephemeralContext);

    public void Force()
    {
        if (CurrentState != State.Registered) return;

        CurrentState = State.Forced;
        _shouldForceFunc = null;
        WebsocketHandler.Instance!.Send(new ActionsForce(_forceQueryGetter!(), _forceStateGetter!(), _forceEphemeralContext,_actions));
    }
    #endregion

    #region End
    
    private Func<bool>? _shouldEndFunc;

    public ActionWindow SetEnd(Func<bool> shouldEnd)
    {
        if (!ValidateFrozen()) return this;

        _shouldEndFunc = shouldEnd;

        return this;
    }

    public ActionWindow SetEnd(float afterSeconds)
    {
        float time = afterSeconds;

        return SetEnd(ShouldEnd);

        bool ShouldEnd()
        {
            time -= Time.GetDelta();;
            return time <= 0;
        }
    }

    private void OnDestroy()
    {
        if (CurrentState == State.Ended) return;
        End();
    }

    public void End()
    {
        if (CurrentState >= State.Ended) return;
        
        NeuroActionHandler.UnregisterActions(_actions);
        _shouldForceFunc = null;
        _shouldEndFunc = null;
        CurrentState = State.Ended;
        //TODO: find way to remove Instance
        return;
    }
    #endregion

    #region Handling    
    public ExecutionResult Result(ExecutionResult result)
    {
        if (CurrentState <= State.Building)
            throw new InvalidOperationException("Cannot handle a result before registering the action window");
        if (CurrentState >= State.Ended)
            throw new InvalidOperationException("Cannot handle a result after the Action window has ended");
        
        if (result.Successful) End();

        return result;
    }
    #endregion
}