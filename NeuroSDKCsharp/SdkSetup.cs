using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp;

public static class SdkSetup
{
    public static void Initialize(string game,string uriString)
    {
        WebsocketHandler ws = new WebsocketHandler(game,uriString);
	    ws.Initialize();
    }
}