using UnityEngine;

using Canopy;
public class StopInPlaceNode : ActionNode
{
    AINavigation navigation = null;
    protected override void OnStart()
    {
        if (navigation == null)
            navigation = blackboard.GetEntry<AINavigation>(blackboard.GetOwnerName() + "Nav");
    }
    protected override void OnStop()
    {
    }
    protected override State OnUpdate()
    {
        navigation.SetDestination(navigation.currentPos);
        return State.Success;
    }
}