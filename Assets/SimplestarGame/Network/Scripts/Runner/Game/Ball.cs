using Fusion;
using UnityEngine;

namespace SimplestarGame
{
    public class Ball : NetworkBehaviour
    {
        [SerializeField] float resetDistance = 50f;

		public override void Spawned()
		{
			this.name = "[Network]Ball";
            
        }

        void Start()
        {
            if (HasStateAuthority)
            {
                this.startPoint = this.transform.position;
            }
        }

        void Update()
        {
            if (!HasStateAuthority)
            {
                return;
            }
            if (this.resetDistance < Vector3.Distance(this.startPoint, this.transform.position))
            {
                this.transform.position = this.startPoint;
                if (this.TryGetComponent(out Rigidbody rigidbody))
                {
                    rigidbody.velocity = Vector3.zero;
                }
            }
        }

        Vector3 startPoint;
    }
}