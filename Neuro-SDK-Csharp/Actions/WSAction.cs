using System.Security.Cryptography;
using Neuro_SDK_Csharp.Json;
using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Actions;

public readonly struct WSAction
{
    public WSAction(string name, string description, JsonSchema? schema)
    {
        Name = name;
        _description = description;
        _schema = schema;
    }

    public readonly string Name;

    private readonly string _description;

    private readonly JsonSchema? _schema;
}