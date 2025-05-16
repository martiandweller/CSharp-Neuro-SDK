using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Neuro_SDK_Csharp.Websocket;

public class WebsocketHandler
{
    private ClientWebSocket _webSocket = new ClientWebSocket();
    private string? _uriString = "";
    private Uri _uri;

    public string Game = ""; // will be used for Messages
    
    public async void StartServer(string urlScheme,string urlHost, string urlPort)
    {
        if (String.IsNullOrEmpty(_uriString))
        {
            try
            {
                string uriSchema = $"{urlScheme}://{urlHost}:{urlPort}/$env/NEURO_SDK_WS_URL";
                _uri = new Uri(uriSchema);
            }
            catch (Exception e)
            {
                // should be fine
            }
        }

        await _webSocket.ConnectAsync(_uri,CancellationToken.None);
        
    }
}