using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Canopy;
public class SequencerNode : CompositeNode
{
    public bool allChildrenMustSucceed = true;
    int doneCount = 0;
    protected override void OnStart()
    {
        doneCount = 0;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        var child = children[doneCount];
        var state = child.Update();
        if (state == State.Success || (!allChildrenMustSucceed && state == State.Failure))
        {
            doneCount++;
            if (doneCount > children.Count - 1)
                return State.Success;
            return State.Running;
        }
        else if (state == State.Failure && allChildrenMustSucceed)
            return State.Failure;

        return State.Running;
    }
}
