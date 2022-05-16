using UnityEditor;
using UnityEngine;

namespace Canopy
{

    [CreateAssetMenu()]
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Failure,
            Success,

        }
        [HideInInspector] public Blackboard blackboard = null;
        [HideInInspector] public State state = State.Running;
        public bool started = false;
        [HideInInspector] public string guid = "";
        [HideInInspector] public Vector2 position;
        //used to label node execution order in editor
        [HideInInspector] public int childIndex = 1;
        [HideInInspector] public Node parent = null;
        public string title = "";

        public State Update()
        {
            if (!started)
                OnStart();
            started = true;

            state = OnUpdate();
            if (state == State.Success || state == State.Failure)
            {
                OnStop();
                started = false;
            }
            return state;
        }
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();

        public virtual Node Clone()
        {
            return Instantiate(this);
        }
    }
}