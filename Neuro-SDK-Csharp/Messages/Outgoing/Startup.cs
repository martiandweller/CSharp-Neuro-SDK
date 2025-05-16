using Neuro_SDK_Csharp.Messages.API;

namespace Neuro_SDK_Csharp.Messages.Outgoing;

public sealed class Startup : OutgoingMessageHandler
{
    protected override string Command => "startup";
    protected override object? Data => null;
}