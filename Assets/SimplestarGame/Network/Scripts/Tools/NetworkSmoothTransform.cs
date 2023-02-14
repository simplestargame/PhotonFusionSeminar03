using Fusion;
using UnityEngine;

namespace SimplestarGame
{
    public class NetworkSmoothTransform : NetworkBehaviour
    {
        [SerializeField, Range(1f, 100f)]
        float smooth = 10f;
        [SerializeField, Range(0.01f, 0.5f)]
        float warpDistance = 0.5f;

        [Networked, HideInInspector]
        public Vector3 position { get; set; }
        [Networked, HideInInspector]
        public Quaternion rotation { get; set; }

        public override void Render()
        {
            base.Render();
            if (HasStateAuthority)
            {
                this.position = this.transform.position;
                this.rotation = this.transform.rotation;
            }
            else if(IsProxy)
            {
                if (this.warpDistance < Vector3.Distance(this.position, this.transform.position))
                {
                    this.transform.position = this.position;
                    this.transform.rotation = this.rotation;
                }
                this.transform.position = Vector3.SmoothDamp(this.transform.position, this.position, ref this.currentVelocity, this.smooth * Time.deltaTime);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.rotation, smooth * Time.deltaTime);
            }
        }

        Vector3 currentVelocity;
    }
}