using Neuro_SDK_Csharp.Messages.API;
using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Messages.Incoming;

public class Action : InterIncomingMessageHandler
{
    public virtual ExecutionResult Validate()
    {
        return new ExecutionResult(true, "asd");
    }
    
    private bool CanHandle(string command)
    {
        return command == "action";
    }
}