using UnityEngine;

using Canopy;
public class MoveToSpottedPosNode : ActionNode
{
    AINavigation navigation = null;
    protected override void OnStart()
    {
        if (navigation == null)
            navigation = blackboard.GetEntryFromThis<AINavigation>("Nav");

        navigation.SetDestination(blackboard.GetEntry<Vector3>("spottedPos"));
    }
    protected override void OnStop()
    {
    }
    protected override State OnUpdate()
    {
        if (!navigation.IsMoving())
            return State.Success;
        return State.Running;
    }
}