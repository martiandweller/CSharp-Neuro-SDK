using Neuro_SDK_Csharp.Websocket;

namespace Neuro_SDK_Csharp.Messages.API;

public class OutgoingMessageHandler
{
    protected virtual string Command => "";
    protected virtual object? Data => this;

    public virtual bool Merge(OutgoingMessageHandler other) => false;

    public WsMessage GetWsMessage() => new(Command, Data, WebsocketHandler.Instance!.Game ??
    throw new InvalidOperationException("Cannot get WsMessage without a Websocket Instance"));
}