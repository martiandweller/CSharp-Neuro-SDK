namespace Neuro_SDK_Csharp.Actions;

public class NeuroActionHandler
{
    enum State
    {
        Building,
        Registered,
        Forced,
        Ended,
    }

    public static bool Validate()
    {
        return true;
    }
}