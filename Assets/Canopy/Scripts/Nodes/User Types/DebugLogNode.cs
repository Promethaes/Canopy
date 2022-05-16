using UnityEngine;

using Canopy;
public class DebugLogNode : ActionNode
{
    public string message;
    protected override void OnStart()
    {
        Debug.Log(message);
    }
    protected override void OnStop()
    {
    }
    protected override State OnUpdate()
    {
        return State.Success;
    }
}