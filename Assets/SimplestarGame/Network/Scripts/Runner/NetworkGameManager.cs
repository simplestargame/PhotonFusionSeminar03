using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace SimplestarGame
{
    [RequireComponent(typeof(NetworkRunner))]
    public class NetworkGameManager : SimulationBehaviour, IPlayerJoined, IPlayerLeft
    {
		[SerializeField] NetworkGame networkGame;
		[SerializeField] NetworkPlayer networkPlayer;

		void IPlayerJoined.PlayerJoined(PlayerRef playerRef)
		{
			if (!this.Runner.IsServer)
            {
				return;
			}
			if (!this.networkGameSpawned)
			{
				this.Runner.Spawn(this.networkGame);
				this.networkGameSpawned = true;
			}
			var networkPlayer = this.Runner.Spawn(this.networkPlayer, inputAuthority: playerRef); 
			this.networkPlayers.Add(playerRef, networkPlayer);
			this.Runner.SetPlayerObject(playerRef, networkPlayer.Object);
		}

		void IPlayerLeft.PlayerLeft(PlayerRef playerRef)
		{
			if (!this.Runner.IsServer)
            {
				return;
            }
			if (!this.networkPlayers.TryGetValue(playerRef, out NetworkPlayer networkPlayer))
            {
				return;
            }
			this.Runner.Despawn(networkPlayer.Object);
			this.networkPlayers.Remove(playerRef);
		}

		Dictionary<PlayerRef, NetworkPlayer> networkPlayers = new Dictionary<PlayerRef, NetworkPlayer>(200);
		bool networkGameSpawned = false;
	}
}
