using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp;

public static partial class SdkSetup
{
    public static async void Initialize(Game gameClass,string game,string uriString)
    {
        WebsocketHandler ws = new WebsocketHandler(gameClass,game,uriString);
        gameClass.Components.Add(ws);
    }
}