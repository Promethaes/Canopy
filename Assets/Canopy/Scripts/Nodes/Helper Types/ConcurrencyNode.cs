using UnityEngine;
using Canopy;

//run all children at once, if any fail, exit
public class ConcurrencyNode : CompositeNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        State childrenState = State.Running;
        int successCount = 0;
        foreach (var child in children)
        {
            childrenState = child.Update();
            if(childrenState == State.Success)
                successCount++;
            else if(childrenState == State.Failure)
                return State.Failure;
            
            if(successCount == children.Count)
                return State.Success;
        }
        return State.Running;
    }
}