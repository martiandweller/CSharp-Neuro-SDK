using System.Security.AccessControl;
using System.Text.Json.Serialization.Metadata;
using Microsoft.VisualBasic.CompilerServices;
using Neuro_SDK_Csharp.Actions;
using Neuro_SDK_Csharp.Messages.API;
using Neuro_SDK_Csharp.Websocket;
using Newtonsoft.Json.Linq;

namespace Neuro_SDK_Csharp.Messages.Incoming;

public class Action : IncomingMessageHandler<Action.ResultData>
{
    public class ResultData
    {
        public ResultData(string id) => Id = id;

        public string Id;
        public INeuroAction? Action;
        public object? Data;
    }
    
    public override bool CanHandle(string command)
    {
        return command == "action";
    }

    protected override ExecutionResult Validate(string command, IncomingData incomingData, out ResultData? resultData)
    {
        if (incomingData.Data == null)
        {
            resultData = null;
            return ExecutionResult.ServerFailure("The action failed as there is no data.");
        }

        string? actionId = incomingData.Data["id"]?.Value<string>();
        
        Console.WriteLine($"this is actionID: {actionId}");
        
        if (string.IsNullOrEmpty(actionId))
        {
            resultData = null;
            return ExecutionResult.ServerFailure("The action failed as there is not ID");
        }

        resultData = new ResultData(actionId);
        try
        {
            Console.WriteLine($"{incomingData.Data}");
            
            string? actionName = incomingData.Data?.Value<string>("name");
            string? actionStringifiedData = incomingData.Data?.Value<string>("data");

            if (string.IsNullOrEmpty(actionName))
            {
                resultData = null;
                return ExecutionResult.ServerFailure("Action has failed as there is no name");
            }

            NeuroActionHandler action = new();
            
            INeuroAction? registeredAction = NeuroActionHandler.GetRegistered(actionName);
            
            Console.WriteLine($"registered action: {registeredAction}");
            
            NeuroActionHandler.RegisterActions();
            
            if (registeredAction == null)
            {
                if (NeuroActionHandler.IsRecentlyUnregistered(actionName))
                {
                    return ExecutionResult.Failure("Action failed unregister");
                }

                return ExecutionResult.Failure("action failed unknown action");
            }

            resultData.Action = registeredAction;
            
            if (!ActionData.TryParse(actionStringifiedData, out ActionData? actionData))
                return ExecutionResult.Failure("Action Failed Invalid JSON");

            ExecutionResult actionValidationResult =
                registeredAction.Validate(actionData!, out object? resultActionData);
            resultData.Data = resultActionData;

            return actionValidationResult;
        }
        catch (Exception e)
        {
            Console.WriteLine($"action failed caught {e}");

            return ExecutionResult.Failure("Action Failed Caught");
        }
    }

    protected override void ReportResult(ResultData? resultData, ExecutionResult executionResult)
    {
        if (resultData == null)
        {
            ExecutionResult.Failure("Action.ReportResult received null data.");
            return;
        }
        
        // send websocket stuff
    }

    protected override Task Execute(ResultData? incomingData)
    {
        return incomingData!.Action!.Execute(incomingData.Data);
    }
}