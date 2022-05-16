using System.Collections.Generic;
using UnityEngine;

namespace Canopy
{

    public class BehaviourTreeController : MonoBehaviour
    {
        public BehaviourTree behaviourTree;
        public Blackboard blackboard;

        private void Awake()
        {
            behaviourTree = behaviourTree.Clone();
            behaviourTree.SetBlackboard(blackboard);
            blackboard.SetBlackboardName(gameObject.name);
        }

        private void Update()
        {
            behaviourTree.Update();
        }
    }

}