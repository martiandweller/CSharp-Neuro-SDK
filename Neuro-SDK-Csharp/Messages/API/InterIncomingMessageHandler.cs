using Neuro_SDK_Csharp.Messages.API;
using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Messages.Incoming;

public class InterIncomingMessageHandler // will server the same purpose of Incoming Message in the godot sdk
{
    public interface IIncomingMessageHandler
    {
        public ExecutionResult Validate(string command,IncomingData incomingData);
        public void ReportResult(ExecutionResult executionResult);
        public Task Execute();
        public bool CanHandle(string command);
    }
}