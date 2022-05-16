using UnityEngine;
using Canopy;
public class WaitNode : ActionNode {
    
    public float duration = 1.0f;
    float startTime = 0.0f;

    protected override void OnStart()
    {
        startTime = Time.time;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return Time.time - startTime > duration ? State.Success : State.Running;
    }
}