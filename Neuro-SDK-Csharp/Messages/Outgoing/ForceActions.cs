using Neuro_SDK_Csharp.Messages.API;

namespace Neuro_SDK_Csharp.Messages.Outgoing;

public class ForceActions : OutgoingMessageHandler
{
    protected override string Command => "actions/force";
}