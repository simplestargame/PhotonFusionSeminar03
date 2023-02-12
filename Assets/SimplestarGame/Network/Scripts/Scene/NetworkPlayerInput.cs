using Fusion;
using Fusion.Sockets;
using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimplestarGame
{
    public struct PlayerInput : INetworkInput
    {
        public Vector2 move;
        public Vector2 look;
        public NetworkBool jump;
        public NetworkBool sprint;
        public float cameraEulerY;
        public Vector3 position;
        public Quaternion rotation;
    }
    public class NetworkPlayerInput : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField, Tooltip("Local player agent")]
        internal Transform agentTransform;
        [SerializeField, Tooltip("StarterAssetsInputs")]
        StarterAssetsInputs starterAssetsInputs;
        [SerializeField, Tooltip("MainCamera Transform")]
        Transform mainCamera;

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
            input.Set(new PlayerInput
            {
                move = this.starterAssetsInputs.move,
                look = this.starterAssetsInputs.look,
                jump = this.starterAssetsInputs.jump,
                sprint = this.starterAssetsInputs.sprint,
                cameraEulerY = this.mainCamera.eulerAngles.y,
                position = this.agentTransform.position,
                rotation = this.agentTransform.rotation,
            });
        }
#region INetworkRunnerCallbacks
        void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
        {
        }

        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }
        #endregion
    }
}