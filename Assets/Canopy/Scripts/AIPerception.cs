using UnityEngine;
using UnityEngine.Events;

namespace Canopy
{
    public class AIPerception : MonoBehaviour
    {
        [SerializeField] protected LayerMask layerMask;

        [Header("References")]
        [SerializeField] protected Blackboard blackboard = null;

        public UnityEvent OnPerceive;

        private void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.layer & ~layerMask) == 0)
                return;
            WriteToBlackboard(other);
        }

        protected virtual void WriteToBlackboard(Collider other)
        {
            blackboard.UpdateEntry("spottedPos", other.gameObject.transform.position);
            OnPerceive.Invoke();
        }
    }
}