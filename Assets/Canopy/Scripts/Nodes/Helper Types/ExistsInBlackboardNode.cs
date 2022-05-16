using Canopy;

public class ExistsInBlackboardNode : DecoratorNode
{
    public string query;
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (blackboard.GetEntry<object>(query) == null)
            return State.Failure;
        return child.Update();
    }
}