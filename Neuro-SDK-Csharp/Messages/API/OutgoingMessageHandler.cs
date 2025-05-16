namespace Neuro_SDK_Csharp.Messages.API;

public class OutgoingMessageHandler
{
    protected virtual string Command => "";
    protected virtual object? Data => this;
}