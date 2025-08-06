using NeuroSDKCsharp.Websocket;

namespace NeuroSDKCsharp.Actions;

    public abstract class NeuroAction : BaseNeuroAction
    {
        protected NeuroAction()
        {
        }

        protected abstract ExecutionResult Validate(ActionData actionData);
        protected abstract void Execute();

        protected sealed override ExecutionResult Validate(ActionData actionData, out object? resultData)
        {
            ExecutionResult result = Validate(actionData);
            resultData = null;
            return result;
        }

        protected sealed override void Execute(object? data) => Execute();
    }

    public abstract class NeuroAction<TData> : BaseNeuroAction
    {
        protected NeuroAction()
        {
        }

        protected abstract ExecutionResult Validate(ActionData actionData, out TData? resultData);
        protected abstract void Execute(TData? resultData);

        protected sealed override ExecutionResult Validate(ActionData actionData, out object? resultData)
        {
            ExecutionResult result = Validate(actionData, out TData? tResultData);
            resultData = tResultData;
            return result;
        }

        protected sealed override void Execute(object? resultData) => Execute((TData?) resultData);
    }

    /// <summary>
    /// NeuroAction with a parsed state with a value type. Use this instead of <see cref="NeuroAction{TData}"/> when using primite types or structs for nullability
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class NeuroActionS<TData> : NeuroAction<TData?> where TData : struct
    {
        protected NeuroActionS()
        {
        }
        
    }
