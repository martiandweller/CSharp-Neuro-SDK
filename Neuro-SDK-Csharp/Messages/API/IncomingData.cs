using Newtonsoft.Json.Linq;

namespace Neuro_SDK_Csharp.Messages.API;

public class IncomingData
{
    public readonly JToken? Data;

    public IncomingData(JToken? data)
    {
        Data = data;
    }
}