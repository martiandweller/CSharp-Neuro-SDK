using System.Security.Cryptography.X509Certificates;
using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp;

public static partial class SdkSetup
{
    public static async void Initialize(string game,string uriString)
    {
        WebsocketHandler ws = new WebsocketHandler();
        ws.Game = game;
        ws.UriString = uriString;
        ws.MessageQueue = new MessageQueue();
        ws.CommandHandler = new CommandHandler();
        await ws.StartWs();
    }
}