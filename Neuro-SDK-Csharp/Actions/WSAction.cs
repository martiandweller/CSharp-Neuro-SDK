using System.Security.Cryptography;
using Neuro_SDK_Csharp.Json;
using Neuro_SDK_Csharp.Websocket;
using Newtonsoft.Json;

namespace Neuro_SDK_Csharp.Actions;

public readonly struct WSAction
{
    public WSAction(string name, string description, JsonSchema? schema)
    {
        Name = name;
        _description = description;
        _schema = schema;
    }

    [JsonProperty("name", Order = 0)]
    public readonly string Name;

    [JsonProperty("description", Order = 10)]
    private readonly string _description;

    [JsonProperty("schema", Order = 20)]
    private readonly JsonSchema? _schema;
}