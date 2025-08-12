using Microsoft.Xna.Framework;
using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp;

public static class SdkSetup
{
    public static void Initialize(Game gameClass,string game,string uriString)
    {
        WebsocketHandler ws = new WebsocketHandler(gameClass,game,uriString);
        gameClass.Components.Add(ws);
    }
}