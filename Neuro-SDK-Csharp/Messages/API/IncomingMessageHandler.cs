using Neuro_SDK_Csharp.Messages.Incoming;
using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Messages.API;

public abstract class IncomingMessageHandler : InterIncomingMessageHandler
{
    public abstract ExecutionResult Validate(string command,IncomingData incomingData);
    public abstract bool CanHandle(string command);
    public abstract void ReportResult(ExecutionResult executionResult);
    public abstract Task Execute();
}