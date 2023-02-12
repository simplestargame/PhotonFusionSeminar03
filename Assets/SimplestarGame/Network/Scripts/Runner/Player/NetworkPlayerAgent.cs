using Fusion;
using StarterAssets;
using UnityEngine;

namespace SimplestarGame
{
	public class NetworkPlayerAgent : NetworkBehaviour
	{
		public NetworkPlayer Owner { get; set; }

		public override void Spawned()
		{
			this.name = "[Network]PlayerAgent:" + Object.InputAuthority.PlayerId;
			this.starterAssetsInputs = this.GetComponent<StarterAssetsInputs>();
			if (null == this.mainCamera)
			{
				this.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
            if (HasInputAuthority)
            {
				var renderers = this.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
					renderer.enabled = false;
                }
                if (this.TryGetComponent(out ThirdPersonController controller))
                {
					controller.FootstepAudioVolume = 0;
                }
			}
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			this.Owner = null;
		}

		public override sealed void FixedUpdateNetwork()
		{
			if (!IsProxy)
            {
				return;
            }
            if (null == this.starterAssetsInputs || null == this.Owner)
            {
				return;
            }
			Vector2 move = this.Owner.StarterAssetsInputs.move;
			if (null != this.mainCamera)
			{
				move = Quaternion.Euler(0, 0, -this.Owner.StarterAssetsInputs.cameraEulerY + this.mainCamera.transform.rotation.eulerAngles.y) * move;
			}
			this.starterAssetsInputs.MoveInput(move);
			this.starterAssetsInputs.LookInput(this.Owner.StarterAssetsInputs.look);
			this.starterAssetsInputs.JumpInput(this.Owner.StarterAssetsInputs.jump);
			this.starterAssetsInputs.SprintInput(this.Owner.StarterAssetsInputs.sprint);
		}

        internal void ApplyInput(PlayerInput input)
        {
			Vector2 move = input.move;
			if (null != this.mainCamera && null != this.Owner)
            {
				move = Quaternion.Euler(0, 0, -this.Owner.StarterAssetsInputs.cameraEulerY + this.mainCamera.transform.rotation.eulerAngles.y) * move;
            }
            
            this.starterAssetsInputs.MoveInput(move);
            this.starterAssetsInputs.LookInput(input.look);
            this.starterAssetsInputs.JumpInput(input.jump);
            this.starterAssetsInputs.SprintInput(input.sprint);
            if (0.5f < Vector3.Distance(this.transform.position, input.position))
            {
				this.transform.position = input.position;
			}
			this.transform.rotation = input.rotation;
		}

		GameObject mainCamera;
		StarterAssetsInputs starterAssetsInputs;
	}
}
