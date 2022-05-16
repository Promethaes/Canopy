using UnityEngine;

using Canopy;
using UnityEngine.AI;

public class MoveToRandomNearbyPosNode : ActionNode
{
    public float length = 5.0f;
    public float edgeAvoidance = 5.0f;
    AINavigation navigation = null;
    protected override void OnStart()
    {
        if (navigation == null)
            navigation = blackboard.GetEntry<AINavigation>(blackboard.GetOwnerName() + "Nav");


        //Find furthest random nearby point
        float mag = float.NegativeInfinity;
        var dest = new Vector3();
        for (int i = 0; i < 30; i++)
        {
            var randomPoint = navigation.currentPos + Random.insideUnitSphere * length;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, length, NavMesh.AllAreas)
            && hit.distance > mag)
            {
                //avoid going to the edge of the navmesh
                NavMeshHit edgeHit;
                if (NavMesh.FindClosestEdge(hit.position, out edgeHit, NavMesh.AllAreas)
                && edgeHit.distance <= edgeAvoidance)
                    continue;

                mag = hit.distance;
                dest = hit.position;
            }
        }

        if (dest.magnitude == 0)
            dest = navigation.currentPos;

        navigation.SetDestination(dest);
    }
    protected override void OnStop()
    {
    }
    protected override State OnUpdate()
    {
        if (navigation.currentDestination.magnitude == 0)
            return State.Failure;
        if (!navigation.IsMoving())
            return State.Success;
        return State.Running;
    }
}