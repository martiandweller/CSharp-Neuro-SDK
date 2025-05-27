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
        Console.WriteLine($"Before Start     {Handlers}");
        try
        {
            Start();
        
            Console.WriteLine($"Handler:  {Handlers}");
        
            foreach (IIncomingMessageHandler handler in Handlers)
            {
                Console.WriteLine($"Running CommandHandler foreach    {handler}");
                if (!handler.CanHandle(command)) continue;

                ExecutionResult validationResult;
                object? resultData;
                try
                {
                    Console.WriteLine($"CommandHandler running validation");
                    validationResult = handler.Validate(command, data, out resultData);
                }
                catch (Exception e)
                {
                    validationResult = ExecutionResult.Failure($"Issue with message handling {e.Message}");
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
                    Console.WriteLine($"CommandHandler validation successful");
                    _ = handler.Execute(resultData);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Issue in Handle \n  {e}");
            throw;
        }
        
    }
}