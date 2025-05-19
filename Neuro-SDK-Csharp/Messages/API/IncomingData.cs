using Newtonsoft.Json.Linq;

namespace Neuro_SDK_Csharp.Messages.API;

public readonly struct IncomingData
{
    public readonly JToken? Data;

    public IncomingData(JToken? data)
    {
        Data = data;
    }
}