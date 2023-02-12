using Fusion;
using UnityEngine;

namespace SimplestarGame
{
	public class NetworkGame : NetworkBehaviour
	{
		[SerializeField] GameObject seeSawPrefab;
		[SerializeField] Transform[] seeSawTransforms;
		[SerializeField] GameObject ballPrefab;
		[SerializeField] Transform[] ballTransforms;

		[Networked, HideInInspector, Capacity(200)]
		public NetworkDictionary<PlayerRef, NetworkPlayer> Players { get; }

		public void Join(NetworkPlayer player)
		{
			if (!HasStateAuthority)
            {
				return;
			}
			var playerRef = player.Object.InputAuthority;
			if (this.Players.ContainsKey(playerRef))
			{
				Debug.LogError($"Player {playerRef} already joined");
				return;
			}
			this.Players.Add(playerRef, player);
			this.SpawnPlayerAgent(player);
		}

		public void Leave(NetworkPlayer player)
		{
			if (!HasStateAuthority)
			{
				return;
			}
			if (!this.Players.ContainsKey(player.Object.InputAuthority))
            {
				return;
            }
			this.Players.Remove(player.Object.InputAuthority);
			this.DespawnPlayerAgent(player);
		}

		public override void Spawned()
		{
			this.name = "[Network]Game";
			NetworkSceneContext.Instance.Game = this;
			Runner.AddCallbacks(NetworkSceneContext.Instance.PlayerInput);
			this.SpawnSeesaws();
			this.SpawnBall();
			if (null != NetworkSceneContext.Instance.hostClientText)
			{
				NetworkSceneContext.Instance.hostClientText.text = Runner.IsServer ? "Host" : "Client";
			}
			Physics.IgnoreLayerCollision(7, 8, true);
		}

        void FixedUpdate()
        {
			if (null != NetworkSceneContext.Instance.countText)
			{
				NetworkSceneContext.Instance.countText.text = $"PlayerCount: {this.Players.Count}";
			}
		}

        public override void Despawned(NetworkRunner runner, bool hasState)
		{
			NetworkSceneContext.Instance.Game = null;
		}

		protected void SpawnPlayerAgent(NetworkPlayer player)
		{
			this.DespawnPlayerAgent(player);

			var agent = this.SpawnAgent(player.Object.InputAuthority, player.AgentPrefab, player.Information.Position, player.Information.Rotation);
			player.AssignAgent(agent);
		}

		protected void DespawnPlayerAgent(NetworkPlayer player)
		{
			if (null == player.ActiveAgent)
			{
				return;
			}

			this.DespawnAgent(player.ActiveAgent);
			player.ClearAgent();
		}

		NetworkPlayerAgent SpawnAgent(PlayerRef inputAuthority, NetworkPlayerAgent agentPrefab, Vector3 spawnPosition, Quaternion spawnRotation)
		{
			var agent = Runner.Spawn(agentPrefab, spawnPosition, spawnRotation, inputAuthority);
			return agent;
		}

		void SpawnSeesaws()
        {
            if (!HasStateAuthority)
            {
				return;
            }
            foreach (var seeSawPoint in this.seeSawTransforms)
            {
				Runner.Spawn(this.seeSawPrefab, seeSawPoint.position, seeSawPoint.rotation);
			}
		}

		void SpawnBall()
        {
			if (!HasStateAuthority)
			{
				return;
			}
			foreach (var ballPoint in this.ballTransforms)
			{
				Runner.Spawn(this.ballPrefab, ballPoint.position, ballPoint.rotation);
			}
		}

		void DespawnAgent(NetworkPlayerAgent agent)
		{
			if (null == agent)
            {
				return;
			}
			Runner.Despawn(agent.Object);
		}
	}
}
