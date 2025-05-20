using System.Net.Sockets;
using Neuro_SDK_Csharp.Messages.API;
using Neuro_SDK_Csharp.Utilities;

namespace Neuro_SDK_Csharp.Websocket;

public class CommandHandler
{
    protected readonly List<IIncomingMessageHandler> Handlers = new();

    public virtual void Start() // this will need to be called manually as there is nothing similar
    {
        Handlers.AddRange(ReflectionHelpers.GetAllInDomain<IIncomingMessageHandler>());
    }

    public virtual void Handle(string command, IncomingData data)
    {
        foreach (IIncomingMessageHandler handler in Handlers)
        {
            if (!handler.CanHandle(command)) continue;

            ExecutionResult validationResult;
            object? resultData;
            try
            {
                validationResult = handler.Validate(command, data, out resultData);
            }
            catch (Exception e)
            {
                validationResult = ExecutionResult.Failure("Big issue at websocket.");
                Console.WriteLine(e);
                resultData = null;  
            }

            if (!validationResult.Successful)
            {
                Console.WriteLine("Unsuccessful execution result when handling message");
            }
            
            handler.ReportResult(resultData, validationResult);

            if (validationResult.Successful)
            {
                _ = handler.Execute(resultData);
            }
        }
    }
}