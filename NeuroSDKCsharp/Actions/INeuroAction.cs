using NeuroSDKCsharp.Messages.API;
using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp.Actions;

public interface INeuroAction
{
    string Name { get; }

    ActionWindow? ActionWindow { get; }

    public bool CanAddToActionWindow(ActionWindow actionWindow);

    Task Execute(object? data);

    WSAction GetWsAction();

    void SetActionWindow(ActionWindow actionWindow);
    
    ExecutionResult Validate(ActionData actionData, out object? data);
}