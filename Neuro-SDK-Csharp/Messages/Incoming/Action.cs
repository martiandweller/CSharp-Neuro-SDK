using Neuro_SDK_Csharp.Actions;
using Neuro_SDK_Csharp.Messages.API;
using Neuro_SDK_Csharp.Websocket;

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
        resultData = null;
        return ExecutionResult.Success("asdf");
    }

    //TODO: Implement these
    protected override void ReportResult(ResultData? resultData, ExecutionResult executionResult)
    {
        throw new NotImplementedException();
    }

    protected override Task Execute(ResultData? incomingData)
    {
        throw new NotImplementedException();
    }
}