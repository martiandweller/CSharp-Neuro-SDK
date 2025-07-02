using System.Security.Cryptography.X509Certificates;
using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp;

public static partial class SdkSetup
{
    public static async void Initialize(string game)
    {
        WebsocketHandler ws = new WebsocketHandler();
        ws.Game = game;
        ws.MessageQueue = new MessageQueue();
        ws.CommandHandler = new CommandHandler();
        await ws.StartWs();
    }
}