using NeuroSDKCsharp.Messages.API;
using NeuroSDKCsharp.Utilities;
using NeuroSDKCsharp.Utilities.Logging;
using System.Diagnostics;

namespace NeuroSDKCsharp.Websocket;

public class CommandHandler
{
    private readonly List<IIncomingMessageHandler> _handlers = new();

    private void Start() // this will need to be called manually as there is nothing similar
    {
        _handlers.AddRange(ReflectionHelpers.GetAllInDomain<IIncomingMessageHandler>());
    }

    public virtual void Handle(string command, IncomingData data)
    {
        if (_handlers.Count == 0) Start();
        try
        {
            foreach (IIncomingMessageHandler handler in _handlers)
            {
                Log.LogTrace($"Running CommandHandler foreach    {handler}");
                if (!handler.CanHandle(command)) continue;

                ExecutionResult validationResult;
                object? resultData;
                try
                {
                    Log.LogTrace($"CommandHandler running validation");
                    validationResult = handler.Validate(command, data, out resultData);
                }
                catch (Exception e)
                {
                    validationResult = ExecutionResult.Failure($"Issue with message handling {e.Message}");
                    Log.LogError(e.Message);
                    resultData = null;  
                }

                if (!validationResult.Successful)
                {
                    Log.LogError("Unsuccessful execution result when handling message");
                }
            
                handler.ReportResult(resultData, validationResult);

                if (validationResult.Successful)
                {
                    Log.LogTrace($"CommandHandler validation successful");
                    handler.Execute(resultData);
                }
            }
        }
        catch (Exception e)
        {
            Log.LogError($"Issue in Handle \n  {e}");
            throw;
        }
        
    }
}