using Newtonsoft.Json;

namespace Neuro_SDK_Csharp.Json;

internal static class JsonSerialize
{
    public static string Serialize(object? value)
    {
        return JsonConvert.SerializeObject(value, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
    }
}