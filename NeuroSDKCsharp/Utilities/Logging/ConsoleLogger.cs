namespace NeuroSDKCsharp.Utilities.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void LogDebug(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(string message)
        {
            Console.WriteLine(message);
        }

        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }

        public void LogTrace(string message)
        {
            Console.WriteLine(message);
        }

        public void LogWarn(string message)
        {
            Console.WriteLine(message);
        }
    }
}
