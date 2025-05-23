using Xunit;
using Neuro_SDK_Csharp;
using Neuro_SDK_Csharp.Messages.API;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Tests;

public class IncomingJsonTests
{
    [Fact]
    public void ProcessMessage_ValidJson_ShouldParseSuccessfully()
    {
        var testingClass = new Neuro_SDK_Csharp.Websocket.WebsocketHandler();
        
        string testJson = "{\"command\": \"action\", \"data\": {\"id\": \"123\", \"name\": \"name\", \"Description\": \"This is Description\"}}";

        Dictionary<string,Object>? result = testingClass.ProcessJsonMessage(testJson);

        
        Assert.NotNull(result);
        IncomingData incomingData = (IncomingData)result["data"];
        Assert.Equal("action", result["command"]);
        Assert.Equal("123", incomingData.Data!["id"]);
        Assert.Equal("action", result["command"]);
    }
    
    [Fact]
    public void ProcessMessage_ValidJson_ShouldParseUnsuccessfully()
    {
        var testingClass = new Neuro_SDK_Csharp.Websocket.WebsocketHandler();
        
        string testJson = "{'command': 'action', 'data': {'id': '123', 'name': 'name', 'Description': 'This is Description'}";

        object? result = testingClass.ProcessJsonMessage(testJson);
        
        Assert.Null(result);
        Assert.IsType<Newtonsoft.Json.JsonReaderException>(result);
    }
}