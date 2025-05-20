using Neuro_SDK_Csharp.Json;
using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Actions;

public abstract class BaseNeuroAction : INeuroAction
{
    public ActionWindow? ActionWindow { get; private set; }

    protected BaseNeuroAction()
    {
        ActionWindow = null;
    }

    public abstract string Name { get; }
    protected abstract string Description { get; }
    protected abstract JsonSchema? Schema { get; }

    public virtual bool CanAddToActionWindow(ActionWindow actionWindow) => true;

    ExecutionResult INeuroAction.Validate(ActionData actionData, out object? ResultData)
    {
        ExecutionResult result = Validate(actionData, out ResultData);

        if (ActionWindow != null)
        {
            // return ActionWindow.Result(result) // not implemented method yet
            return ExecutionResult.Failure("sdafsdaf");
        }

        return result;
    }

    Task INeuroAction.Execute(object? data) => Execute(data);
    
    public virtual WSAction GetWsAction()
    {
        return new WSAction(Name, Description, Schema);
    }

    protected abstract ExecutionResult Validate(ActionData actionData, out object? ResultData);
    protected abstract Task Execute(object? data);

    void INeuroAction.SetActionWindow(ActionWindow actionWindow)
    {
        if (ActionWindow != null)
        {
            if (ActionWindow == actionWindow)
            {
                ExecutionResult.Failure("Cannot set the action window for this action as it is already set");
            }
            
            return;
        }

        ActionWindow = actionWindow;
    }
}