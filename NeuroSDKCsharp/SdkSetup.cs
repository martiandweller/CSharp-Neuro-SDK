using NeuroSDKCsharp.Utilities.Logging;
using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp;

public static class SdkSetup
{
    public static async void Initialize(string game,string uriString, ILogger logger)
    {
        Log.Initialize(logger);
        WebsocketHandler ws = new WebsocketHandler(game,uriString);
	    ws.Initialize();
    }
}