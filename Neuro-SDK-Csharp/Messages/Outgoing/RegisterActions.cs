using Neuro_SDK_Csharp.Messages.API;

namespace Neuro_SDK_Csharp.Messages.Outgoing;

public sealed class RegisterActions : OutgoingMessageHandler
{
    protected override string Command => "actions/register";
}