using Neuro_SDK_Csharp.Messages.API;
using Neuro_SDK_Csharp.Websocket;
using Newtonsoft.Json;

namespace Neuro_SDK_Csharp.Messages.Outgoing;

public sealed class ActionResult : OutgoingMessageHandler
{
    public ActionResult(string id,ExecutionResult result)
    {
        _id = id;
        _success = result.Successful;
        _message = result.Message;
    }
    
    protected override string Command => "action/result";
    
    [JsonProperty("id")]
    private string _id;

    [JsonProperty("success")]
    private bool _success;

    [JsonProperty("message")]
    private string? _message;
}