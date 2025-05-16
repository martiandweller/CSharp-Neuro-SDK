using Neuro_SDK_Csharp.Messages.API;
using Newtonsoft.Json;

namespace Neuro_SDK_Csharp.Messages.Outgoing;

public sealed class ActionResult : OutgoingMessageHandler
{
    public ActionResult(string id,bool success,string? message)
    {
        _id = id;
        _success = success;
        _message = message;
    }
    
    protected override string Command => "action/result";
    
    [JsonProperty("id")]
    private string _id;

    [JsonProperty("success")]
    private bool _success;

    [JsonProperty("message")]
    private string? _message;
}