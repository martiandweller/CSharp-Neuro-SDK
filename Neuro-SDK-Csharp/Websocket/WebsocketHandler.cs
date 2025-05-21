using System.Net.WebSockets;
using System.Text;
using Neuro_SDK_Csharp.Json;
using Neuro_SDK_Csharp.Messages.API;
using Newtonsoft.Json.Linq;

namespace Neuro_SDK_Csharp.Websocket;

public class WebsocketHandler
{
    private const float ReconnectInterval = 3;

    private static WebsocketHandler? _instance;

    public static WebsocketHandler? Instance
    {
        get
        {
            if (_instance is null)
                ExecutionResult.Failure("WebsocketHandlerInstance was accessed without an instance being present");
            return _instance;
        }
        private set => _instance = value;
    }

    //  ws://localhost:8001/ws/

    private ClientWebSocket? _webSocket = new ClientWebSocket();

    public string Game = null!; // will be used for Messages
    public MessageQueue MessageQueue = null!;
    public CommandHandler CommandHandler = null!;

    private string? _uriString = ""; // this will be changed to be able to be changed through file in future
    private Uri _uri;


    private void Start() => StartWs();

    //  string urlScheme, string urlHost, string urlPort

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

        if (websocketUri == null)
        {
            websocketUri = new Uri("ws://localhost:8000/ws/"); // this is temporary
        }

        // if (_uriString is null or "")
        // {
        //     try
        //     {
        //         HttpClient client = new HttpClient();
        //         string responseBody = await client.GetStringAsync(_uriString);
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         throw;
        //     }
        // }

        if (_uriString is null or "")
        {
            _uriString = Environment.GetEnvironmentVariable("NEURO_SDK_WS_URL", EnvironmentVariableTarget.Process) ??
                         Environment.GetEnvironmentVariable("NEURO_SDK_WS_URL", EnvironmentVariableTarget.User) ??
                         Environment.GetEnvironmentVariable("NEURO_SDK_WS_URL", EnvironmentVariableTarget.Machine);
            Console.WriteLine(_uriString);
        }

        _uriString = "ws://localhost:8000/ws/";

        Console.WriteLine($"This is _uriString test: {_uriString}");

        if (_uriString is null or "")
        {
            string errorMessage =
                "Could not get websocket URL. You need to set the NEURO_SDK_WS_URL environment variable";

            Console.WriteLine(errorMessage);
            return;
        }

        _webSocket = new ClientWebSocket();
        websocketUri = new Uri(_uriString);
        
        try
        {
            await _webSocket.ConnectAsync(websocketUri, CancellationToken.None);

            Console.WriteLine($"Starting Task");

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
        string message = JsonSerialize.Serialize(handler.GetWsMessage());

        Console.WriteLine($"Sending the Ws Message {message}");

        var sendBytes = Encoding.UTF8.GetBytes(message);

        try
        {
            await _webSocket!.SendAsync(sendBytes, WebSocketMessageType.Text, false, CancellationToken.None);
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

        try
        {
            Console.WriteLine($"Start of Try");
            
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                Console.WriteLine($"Result: {result} || {result.MessageType}");
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("Server closed connection.");
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
                
                var messageData = Encoding.UTF8.GetString(buffer);

                Console.WriteLine($"Running MessageData");
                GetMessage(messageData);
                Console.WriteLine($"After GetMessage");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Got invalid message \n {e}");
        }
    }

    private void GetMessage(string messageData)
    {
        Console.WriteLine($"Message data: {messageData}");
        try
        {
            Console.WriteLine($"Start Try");
            
            JObject message = JObject.Parse(messageData);
            string? command = message["command"]?.Value<string>();
            IncomingData data = new(message["data"]);
            
            Console.WriteLine($"after variable stuff   Message: {message}  Command: {command}   Data: {data}");
        
            if (command is null)
            {
                Console.WriteLine($"Command could not be deserialized");
                ExecutionResult.Failure("Command could not be deserialized");
            }
        
            Console.WriteLine($"Send to commandhandler");
            CommandHandler.Handle(command!, data);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Catching error in Get message");
        }
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
    