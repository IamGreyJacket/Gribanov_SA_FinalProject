using UnityEngine;

namespace Racer.Managers.Assistants
{
    public class Checkpoint : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var judge = other.GetComponentInParent<Judge>();
            if (judge == null) return;
            judge.UpdateCheckpoint(this, true);
        }
    }
}