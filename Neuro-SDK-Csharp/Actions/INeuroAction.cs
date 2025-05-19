using Neuro_SDK_Csharp.Messages.API;
using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Actions;

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