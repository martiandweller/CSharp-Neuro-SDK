using NeuroSDKCsharp.Utilities.Logging;
using NeuroSDKCsharp.Websocket;
using Newtonsoft.Json;

namespace NeuroSDKCsharp.Json;

internal static class JsonSerialize
{
    public static string Serialize(object? value)
    {
        Log.LogTrace($"inside serialize value: {value}");
        return JsonConvert.SerializeObject(value, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}