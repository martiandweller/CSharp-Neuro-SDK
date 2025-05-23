using Neuro_SDK_Csharp.Websocket;
using Newtonsoft.Json;

namespace Neuro_SDK_Csharp.Json;

internal static class JsonSerialize
{
    public static string Serialize(object? value)
    {
        Console.WriteLine($"inside serialize value: {value}");
        return JsonConvert.SerializeObject(value, new JsonSerializerSettings // this has issue and causes program to stop
        {
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}