using Fusion;
using UnityEngine;

namespace SimplestarGame
{
	public struct PlayerInformation : INetworkStruct
	{
		public PlayerRef PlayerRef;
		public Vector3 Position;
		public Quaternion Rotation;
	}

	public struct StarterAssetInputs : INetworkStruct
	{
		public Vector2 move;
		public Vector2 look;
		public NetworkBool jump;
		public NetworkBool sprint;
		public float cameraEulerY;
	}

	public class NetworkPlayer : NetworkBehaviour, IBeforeTick
	{
		[SerializeField] NetworkPlayerAgent agentPrefab;
		internal NetworkPlayerAgent AgentPrefab => this.agentPrefab;

		[Networked(OnChanged = nameof(OnActiveAgentChanged), OnChangedTargets = OnChangedTargets.All), HideInInspector]
		internal NetworkPlayerAgent ActiveAgent { get; private set; }
		[Networked] 
		internal ref PlayerInformation Information => ref MakeRef<PlayerInformation>();
		[Networked]
		internal ref StarterAssetInputs StarterAssetsInputs => ref MakeRef<StarterAssetInputs>();

		public static void OnActiveAgentChanged(Changed<NetworkPlayer> changed)
		{
			if (null != changed.Behaviour.ActiveAgent)
			{
				changed.Behaviour.AssignAgent(changed.Behaviour.ActiveAgent);
			}
			else
			{
				changed.Behaviour.ClearAgent();
			}
		}

		public void AssignAgent(NetworkPlayerAgent agent)
		{
			if (HasStateAuthority)
			{
                if (this.ActiveAgent == agent)
                {
					return; // Skip Scond Callback
                }
			}

			this.ActiveAgent = agent;
			this.ActiveAgent.Owner = this;

			Transform rendererAgent = agent.transform;
            if (HasInputAuthority)
            {
				rendererAgent = NetworkSceneContext.Instance.PlayerInput.agentTransform;
			}
			int colorIndex = this.Information.PlayerRef.PlayerId % this.playerColors.Length;
			var renderers = rendererAgent.GetComponentsInChildren<Renderer>();
			foreach (var renderer in renderers)
			{
                foreach (var material in renderer.materials)
                {
					material.color = this.playerColors[colorIndex];
				}
			}
		}

		public void ClearAgent()
		{
			if (null == this.ActiveAgent)
            {
				return;
            }
			this.ActiveAgent.Owner = null;
			this.ActiveAgent = null;
		}

		public override void Spawned()
		{
			this.name = "[Network]Player:" + Object.InputAuthority.PlayerId;
			this.Information.PlayerRef = Object.InputAuthority;
			if (NetworkSceneContext.Instance.Game != null)
			{
				NetworkSceneContext.Instance.Game.Join(this);
			}
		}

		void IBeforeTick.BeforeTick()
		{
			if (GetInput(out PlayerInput input))
			{
				this.Information.Position = input.position;
				this.Information.Rotation = input.rotation;
				this.StarterAssetsInputs.move = input.move;
				this.StarterAssetsInputs.look = input.look;
				this.StarterAssetsInputs.jump = input.jump;
				this.StarterAssetsInputs.sprint = input.sprint;
				this.StarterAssetsInputs.cameraEulerY = input.cameraEulerY;
				bool agentValid = this.ActiveAgent != null && this.ActiveAgent.Object != null;
				if (agentValid)
				{
					this.ActiveAgent.ApplyInput(input);
				}
			}
		}

        public override void Despawned(NetworkRunner runner, bool hasState)
		{
			if (!hasState)
            {
				return;
			}

			if (null != NetworkSceneContext.Instance.Game)
			{
				NetworkSceneContext.Instance.Game.Leave(this);
			}
			if (Object.HasStateAuthority && this.ActiveAgent != null)
			{
				Runner.Despawn(this.ActiveAgent.Object);
			}
			this.ActiveAgent = null;
		}

		Color[] playerColors = new Color[] { new Color(0.8f, 0.2f, 0.6f), Color.green, Color.cyan, Color.yellow, Color.red, Color.blue, Color.magenta, Color.grey, Color.black, Color.white };
	}
}
