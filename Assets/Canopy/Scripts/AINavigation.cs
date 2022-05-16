using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Canopy
{
    public class AINavigation : MonoBehaviour
    {

        [Header("References")]
        [SerializeField] NavMeshAgent navMeshAgent = null;
        [SerializeField] protected Blackboard blackboard = null;

        string blackboardName = "";

        [HideInInspector] public Vector3 currentPos { get; private set; }
        [HideInInspector] public Vector3 currentDestination { get; private set; }
        private void Start()
        {
            blackboardName = blackboard.GetEntry<string>("name") + "Nav";
            blackboard.UpdateEntry(blackboardName, this);
            UpdatePositions();
        }

        private void Update()
        {
            UpdatePositions();
        }

        void UpdatePositions()
        {
            currentPos = navMeshAgent.gameObject.transform.position;
            currentDestination = navMeshAgent.destination;
        }

        public void SetDestination(Vector3 dest)
        {
            currentDestination = dest;
            navMeshAgent.SetDestination(dest);
        }

        public float GetRemainingDistance()
        {
            var f = (currentDestination - currentPos);
            f.y = 0.0f;
            return f.magnitude;
        }

        public bool IsMoving()
        {
            return GetRemainingDistance() > Mathf.Epsilon;
        }

        public NavMeshAgent GetNavMeshAgent()
        {
            return navMeshAgent;
        }

    }
}