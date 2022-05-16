using UnityEngine;

namespace Canopy
{

    public class RootNode : DecoratorNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if(!child)
                return State.Failure;
            return child.Update();
        }
    }
}