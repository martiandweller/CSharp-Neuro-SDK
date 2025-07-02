using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp.Messages.API
{
    public interface IIncomingMessageHandler
    {
        ExecutionResult Validate(string command,IncomingData incomingData, out object? resultData);
        void ReportResult(object? resultData,ExecutionResult executionResult);
        Task Execute(object? incomingData);
        bool CanHandle(string command);
    }
    
    public abstract class IncomingMessageHandler : IIncomingMessageHandler
    {
        public abstract bool CanHandle(string command);
        protected abstract ExecutionResult Validate(string command,IncomingData incomingData);
        protected abstract void ReportResult(ExecutionResult executionResult);
        protected abstract Task Execute();
    
        ExecutionResult IIncomingMessageHandler.Validate(string command,IncomingData incomingData, out object? resultData)
        {
            Console.WriteLine($"Before Result");
            ExecutionResult result = Validate(command, incomingData);
            Console.WriteLine($"IncomingMessageHandler Validate Result   {result}");
            resultData = null;
            return result;
        }

        void IIncomingMessageHandler.ReportResult(object? resultData, ExecutionResult executionResult) =>
            ReportResult(executionResult);

        Task IIncomingMessageHandler.Execute(object? incomingData) => Execute();
    }
    
    public abstract class IncomingMessageHandler<T> : IIncomingMessageHandler
    {
        public abstract bool CanHandle(string command);
        protected abstract ExecutionResult Validate(string command,IncomingData incomingData, out T? resultData);
        protected abstract void ReportResult(T? resultData,ExecutionResult executionResult);
        protected abstract Task Execute(T? incomingData);
    
        ExecutionResult IIncomingMessageHandler.Validate(string command,IncomingData incomingData, out object? resultData)
        {
            Console.WriteLine($"Before Result");
            ExecutionResult result = Validate(command, incomingData, out var tResultData);
            Console.WriteLine($"IncomingMessageHandler Result   {result.Message}");
            resultData = tResultData;
            return result;
        }

        void IIncomingMessageHandler.ReportResult(object? resultData, ExecutionResult executionResult) =>
            ReportResult((T?) resultData,executionResult);

        Task IIncomingMessageHandler.Execute(object? incomingData) => Execute((T?) incomingData);
    }
}