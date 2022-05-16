using UnityEngine;
using Canopy;
public class SelectorNode : CompositeNode
{
    int index = 0;
    protected override void OnStart()
    {
        index = Random.Range(0,children.Count);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return children[index].Update();
    }
}