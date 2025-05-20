using Neuro_SDK_Csharp.Actions;
using Neuro_SDK_Csharp.Messages.API;

namespace Neuro_SDK_Csharp.Messages.Outgoing;

public class ActionsForce : OutgoingMessageHandler
{
    protected override string Command => "actions/force";

    public ActionsForce(string query, string? state, bool? ephemeralContext, IEnumerable<INeuroAction> actions)
    {
        _state = state;
        _query = query;
        _ephemeralContext = ephemeralContext;
        _actionNames = actions.Select(a => a.Name).ToArray();
    }

    public ActionsForce(string query, string? state, bool? ephemeralContext, params INeuroAction[] actions) : this(
        query, state, ephemeralContext, (IEnumerable<INeuroAction>)actions)
    {}

    private readonly string? _state;

    private readonly string _query;

    private readonly bool? _ephemeralContext;

    private readonly string[] _actionNames;
}