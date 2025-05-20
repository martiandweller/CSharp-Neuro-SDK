namespace Neuro_SDK_Csharp.Websocket;

public record WsMessage
{
    public WsMessage(string command, object? data, string game)
    {
        Command = command;
        Data = data;
        _game = game;
    }

    public readonly string Command;
    private readonly string _game;
    public readonly object? Data;
}