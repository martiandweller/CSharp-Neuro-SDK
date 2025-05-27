using System.Net.Sockets;
using Neuro_SDK_Csharp.Actions;
using Neuro_SDK_Csharp.Messages.API;
using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Messages.Incoming;

public sealed class ActionsReregisterAll : IncomingMessageHandler
{
    public override bool CanHandle(string command) => command == "actions/reregister_all";
    
    protected override ExecutionResult Validate(string command, IncomingData incomingData) => ExecutionResult.Success();

    protected override void ReportResult(ExecutionResult result)
    {
    }

    protected override Task Execute()
    {
        NeuroActionHandler.ResendRegisteredActions();
        return Task.CompletedTask;
    }
}