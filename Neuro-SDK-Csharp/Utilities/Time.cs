namespace Neuro_SDK_Csharp.Utilities;

public abstract class Time
{
    static DateTime previousTime = DateTime.Now;
    static DateTime currentTime = DateTime.Now;
    
    public static float GetDelta()
    {
        currentTime = DateTime.Now;
        float deltaTime = (currentTime.Ticks - previousTime.Ticks) / 10000000f;
        previousTime = currentTime;
        return deltaTime;
    }
}