using Neuro_SDK_Csharp.Messages.Outgoing;
using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Actions;

public sealed class NeuroActionHandler
{
    private static List<INeuroAction> _currentRegisteredActions = new();
    private static readonly List<INeuroAction> _dyingActions = new();

    public static INeuroAction? GetRegistered(string name) =>
        _currentRegisteredActions.FirstOrDefault(a => a.Name == name);

    public static bool IsRecentlyUnregistered(string name) => _dyingActions.Any(a => a.Name == name);

    private void OnApplicationQuit()
    { 
        WebsocketHandler.Instance!.SendImmediate(new ActionsUnregister(_currentRegisteredActions));
        _currentRegisteredActions = null!;
    }

    public static void RegisterActions(IReadOnlyCollection<INeuroAction> newActions)
    {
        _currentRegisteredActions.RemoveAll(
            oldAction => newActions.Any(newAction => oldAction.Name == newAction.Name));
        _dyingActions.RemoveAll(oldAction => newActions.Any(newAction => oldAction.Name == newAction.Name));
        _currentRegisteredActions.AddRange(newActions);
        WebsocketHandler.Instance!.Send(new ActionsRegister(newActions));
    }

    public static void RegisterActions(params INeuroAction[] newActions)
        => RegisterActions((IReadOnlyCollection<INeuroAction>)newActions);

    public static void UnregisterActions(IEnumerable<string> removeActionsList)
    {
        INeuroAction[] actionsToRemove = _currentRegisteredActions.Where(oldAction =>
            removeActionsList.Any(removeAction => oldAction.Name == removeAction)).ToArray();

        _currentRegisteredActions.RemoveAll(actionsToRemove.Contains);
        _dyingActions.AddRange(actionsToRemove);
        _ = RemoveActions(); // should replicate .Forget with unitask
        
        WebsocketHandler.Instance!.Send(new ActionsUnregister(removeActionsList));
        
        return;

        async Task RemoveActions()
        {
            await Task.Delay(10000);
            _dyingActions.RemoveAll(actionsToRemove.Contains);
        }
    }

    public static void UnregisterActions(IEnumerable<INeuroAction> removeActionsList) =>
        UnregisterActions(removeActionsList.Select(a => a.Name));

    public static void UnregisterActions(params INeuroAction[] removeActinsList) =>
        UnregisterActions((IReadOnlyCollection<INeuroAction>)removeActinsList);

    public static void UnregisterActions(params string[] removeActionsNamesList) =>
        UnregisterActions((IReadOnlyCollection<string>)removeActionsNamesList);

    public static void ResendRegisteredActions()
    {
        WebsocketHandler.Instance!.Send(new ActionsRegister(_currentRegisteredActions));
    }
    
}