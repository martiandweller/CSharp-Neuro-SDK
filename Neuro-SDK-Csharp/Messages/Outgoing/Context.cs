using Neuro_SDK_Csharp.Messages.API;
using Neuro_SDK_Csharp.Websocket;
using Newtonsoft.Json;

namespace Neuro_SDK_Csharp.Messages.Outgoing;

public sealed class Context : OutgoingMessageHandler
{
    public Context(string message, bool silent)
    {
        _message = message;
        _silent = silent;
    }
    
    protected override string Command => "context";

    [JsonProperty("message")]
    private string _message;

    [JsonProperty("silent")]
    private bool _silent;

    public static void Send(string message, bool silent = false) =>
        WebsocketHandler.Instance!.Send(new Context(message, silent));
}
