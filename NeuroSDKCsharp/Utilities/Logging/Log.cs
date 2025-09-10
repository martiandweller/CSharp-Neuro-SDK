namespace NeuroSDKCsharp.Utilities.Logging
{
    public static class Log
    {
        private static ILogger? _logger;

        private static ILogger Instance
        {
            get
            {
                if (_logger is null)
                {
                    throw new InvalidOperationException("Log isn't initialized");
                }
                return _logger;
            }
        }

        public static void Initialize(ILogger logger)
        {
            _logger = logger;
        }

        public static void LogTrace(string message)
        {
            Instance.LogTrace(message);
        }

        public static void LogDebug(string message)
        {
            Instance.LogDebug(message);
        }

        public static void LogInfo(string message)
        {
            Instance.LogInfo(message);
        }

        public static void LogWarn(string message)
        {
            Instance.LogWarn(message);
        }

        public static void LogError(string message)
        {
            Instance.LogError(message);
        }


    }
}
