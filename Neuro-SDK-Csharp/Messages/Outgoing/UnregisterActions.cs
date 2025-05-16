using Neuro_SDK_Csharp.Messages.API;

namespace Neuro_SDK_Csharp.Messages.Outgoing;

public sealed class UnregisterActions : OutgoingMessageHandler
{
    protected override string Command => "actions/unregister";
}