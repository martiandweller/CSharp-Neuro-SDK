using System.Net.WebSockets;
using System.Text;
using Microsoft.Xna.Framework;
using NeuroSDKCsharp.Json;
using NeuroSDKCsharp.Messages.API;
using Newtonsoft.Json.Linq;

namespace NeuroSDKCsharp.Websocket;

public class WebsocketHandler : GameComponent
{
    private const float ReconnectInterval = 30;

    private static WebsocketHandler? _instance;

    public WebsocketHandler(Game game, string gameName, string? uriString) : base(game)
    {
        GameName = gameName;
        MessageQueue = new MessageQueue();
        CommandHandler = new CommandHandler();
        UriString = uriString;
    }

    public static WebsocketHandler? Instance
    {
        get
        {
            if (_instance is null)
                Console.WriteLine("WebsocketHandlerInstance was accessed without an instance being present");
            return _instance;
        }
        private set => _instance = value;
    }

    //  ws://localhost:8001/ws/

    private ClientWebSocket? _webSocket = new ClientWebSocket();

    public readonly string GameName = null!; // will be used for Messages
    private readonly MessageQueue MessageQueue = null!;
    private readonly CommandHandler CommandHandler = new CommandHandler();

    private string? UriString = "ws://localhost:8000/ws/"; // this will be changed to be able to be changed through file in future
    public override async void Initialize()
    {
        try
        {
            await StartWs();
        }
        catch (Exception e)
        {
            Console.WriteLine($"issue in initialize: {e}");
        }
    }
    
    public async Task StartWs()
    {
        Console.WriteLine("This is start of Ws");
        Instance = this;
        
        try
        {
            if (_webSocket!.State is WebSocketState.Open or WebSocketState.Connecting)
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        Uri? websocketUri = null;

        if (UriString is null or "")
        {
            UriString = Environment.GetEnvironmentVariable("NEURO_SDK_WS_URL", EnvironmentVariableTarget.Process) ??
                         Environment.GetEnvironmentVariable("NEURO_SDK_WS_URL", EnvironmentVariableTarget.User) ??
                         Environment.GetEnvironmentVariable("NEURO_SDK_WS_URL", EnvironmentVariableTarget.Machine);
            Console.WriteLine($"UriString: {UriString}");
        }
        
        if (UriString is null or "")
        {
            string errorMessage =
                "Could not get websocket URL. You need to set the NEURO_SDK_WS_URL environment variable";

            Console.WriteLine(errorMessage);
            return;
        }

        _webSocket = new ClientWebSocket();
        websocketUri = new Uri(UriString);

        _webSocket.Options.KeepAliveInterval = TimeSpan.FromMinutes(10); // should substitute ping pong
        
        try
        {
            // need to add reconnect to this
            // if (_webSocket.ConnectAsync(websocketUri, CancellationToken.None) is WebSocketException)
            // {
            //     throw new Exception();
            // }
            await _webSocket.ConnectAsync(websocketUri, CancellationToken.None);

            Console.WriteLine($"Starting Task    Websocket connected {_webSocket.State}");
            
            _ = Task.Run(ReceiveMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task SendTask(OutgoingMessageHandler handler)
    {
        Console.WriteLine($"running SendTask");

        WsMessage wsMessage = handler.GetWsMessage(); 
        
        Console.WriteLine($"wsMessage before serialize {wsMessage}");
        
        string message = JsonSerialize.Serialize(wsMessage);
        
        Console.WriteLine($"Sending the Ws Message {message}");

        var sendBytes = Encoding.UTF8.GetBytes(message);
        
        try
        {
            await _webSocket!.SendAsync(sendBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            MessageQueue.Enqueue(handler);
        }
    }

    public void Send(OutgoingMessageHandler messageHandler) => MessageQueue.Enqueue(messageHandler);

    // async void is bad, but I'm just gonna pray this works
    public async void SendImmediate(OutgoingMessageHandler messageHandler)
    {
        string message = JsonSerialize.Serialize(messageHandler.GetWsMessage());

        if (_webSocket.State is not WebSocketState.Open)
        {
            Console.WriteLine($"Websocket is not open. Could not send message: {message}");
        }

        Console.WriteLine($"Sending Immediate message {message}");

        var sendBytes = Encoding.UTF8.GetBytes(message);
        await _webSocket!.SendAsync(sendBytes, WebSocketMessageType.Text, false, CancellationToken.None);
    }

    private async Task ReceiveMessage()
    {
        Console.WriteLine($"Start of ReceiveMessage");

        var buffer = new byte[1024 * 4];
        
        while (_webSocket.State == WebSocketState.Open)
        {
            Console.WriteLine($"message queue count : {MessageQueue.Count}");
            
            Console.WriteLine($"Current websocket state Start {_webSocket.State}");
            WebSocketReceiveResult result;
            MemoryStream memoryStream = new MemoryStream();
            do
            {
                result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                memoryStream.Write(buffer,0,result.Count);
            } while (!result.EndOfMessage);
            
            Console.WriteLine($"Receive message result: {result} || {result.MessageType}  || {result.CloseStatus}");
            
            if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("Server closed connection.");
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                break;
            }

            var memoryStreamArray = memoryStream.ToArray();
            var messageData = Encoding.UTF8.GetString(memoryStreamArray,0,memoryStreamArray.Length);
            
            Console.WriteLine($"Running MessageData");
            GetMessage(messageData);
            Console.WriteLine($"After GetMessage");
        }

        if (_webSocket.State != WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,"",CancellationToken.None);
        }
    }

    public override async void Update(GameTime gameTime)
    {
        try
        {
            if (_webSocket.State != WebSocketState.Open) return;

            while (MessageQueue.Count > 0)
            {
                OutgoingMessageHandler handler = MessageQueue.Dequeue()!;
                Console.WriteLine(handler);
                await SendTask(handler);
            }
            base.Update(gameTime);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Issue in update of ws: {e}");
        }
    }
    
    private void GetMessage(string messageData)
    {
        Console.WriteLine($"Message data: {messageData}");
        messageData = messageData.Replace("\'", "\"");
        Console.WriteLine($"Message data: {messageData}");
        try
        {
            Dictionary<string, Object>? dataArray = ProcessJsonMessage(messageData);
            if (dataArray is null) return;
            // command data
            CommandHandler.Handle((string)dataArray["command"], (IncomingData)dataArray["data"]);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Catching error in GetMessage try   {e}");
        }
    }

    public Dictionary<string, Object>? ProcessJsonMessage(string messageData)
    {
            // messageData = @"{\""command\"":\""action\"",\""data\"":{\""id\"":\""123\"",\""name\"":\""name\"",\""Description\"":\""This is Description\""}}";
            Console.WriteLine($"Start Try");
            JObject message = JObject.Parse(messageData); // this just doesnt work idk why
            Console.WriteLine($"after message  {message}");
            
            string? command = message["command"]?.Value<string>();
            Console.WriteLine($"after command  {command}");
            IncomingData data = new(message["data"]);

            Console.WriteLine($"after variable stuff   Message: {message}  Command: {command}   Data: {data.Data}");

            if (command is null)
            {
                Console.WriteLine($"Command could not be deserialized");
                ExecutionResult.Failure("Command could not be deserialized");
            }

            Console.WriteLine($"Send to CommandHandler");
            Dictionary<string, Object> dataDictionary = new Dictionary<string, object>{{"message", message},{"command",command},{"data",data}};
            return dataDictionary;
    }
}

// too dumb to make this work

// private bool TryGetResult(HttpRequestMessage request.out string result)
    // {
    //     request.
    //     if (request is { isDone: true, isHttpError: false, isNetworkError: false})
    //     {
    //         result = request.GetResponse();
    //         return true;
    //     }
    //
    //     return false;
    // }
    