using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SimplestarGame
{
    [DisallowMultipleComponent]
    public class NetworkConnectionManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField, Tooltip("Flag to start connection on Awake")]
        bool connectOnAwake = true;
        [SerializeField, Tooltip("Network runner prefab")]
        NetworkRunner networkRunnerPrefab;
        [SerializeField, Tooltip("Blank means you will enter a random room")]
        string sessionName = "";
        [SerializeField, Tooltip("Default server port")]
        ushort serverPort = 27015;

        void Awake()
        {
            if (this.connectOnAwake)
            {
                StartCoroutine(this.CoInstantiateNetworkRunner());
            }
        }

        IEnumerator CoInstantiateNetworkRunner()
        {
            if (!this.networkRunnerPrefab)
            {
                Debug.LogError($"{nameof(this.networkRunnerPrefab)} not set, can't perform network start.");
                yield break;
            }
            NetworkRunner runner = Instantiate(this.networkRunnerPrefab);
            runner.AddCallbacks(this);
            DontDestroyOnLoad(runner);
            runner.name = "[Network]Runner";
#if UNITY_EDITOR
            this.serverPort += 1;
#endif
            if (this.gameObject.transform.parent)
            {
                Debug.LogWarning($"{nameof(NetworkConnectionManager)} can't be a child game object, un-parenting.");
                this.gameObject.transform.parent = null;
            }
            DontDestroyOnLoad(this.gameObject);
            var clientTask = this.InitializeNetworkRunner(runner);
        }

        Task InitializeNetworkRunner(NetworkRunner runner)
        {
            var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
            if (null == sceneManager)
            {
                Debug.Log($"NetworkRunner does not have any component implementing {nameof(INetworkSceneManager)} interface, adding {nameof(NetworkSceneManagerDefault)}.", runner);
                sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }
            this.connectionToken = Guid.NewGuid().ToByteArray();
            return runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                Address = NetAddress.Any(this.serverPort),
                SessionName = this.sessionName,
                SceneManager = sceneManager,
                ConnectionToken = this.connectionToken,
            });
        }

        async void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            await runner.Shutdown(true, ShutdownReason.HostMigration);

            runner = Instantiate(this.networkRunnerPrefab);
            runner.AddCallbacks(this);
            DontDestroyOnLoad(runner);
            runner.name = "[Network]Runner:" + hostMigrationToken.GameMode;
            var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
            if (null == sceneManager)
            {
                Debug.Log($"NetworkRunner does not have any component implementing {nameof(INetworkSceneManager)} interface, adding {nameof(NetworkSceneManagerDefault)}.", runner);
                sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }
            await runner.StartGame(new StartGameArgs
            {
                HostMigrationToken = hostMigrationToken,
                HostMigrationResume = HostMigrationResume,
                SceneManager = sceneManager,
                ConnectionToken = this.connectionToken,
            });
        }

        void HostMigrationResume(NetworkRunner runner)
        {
            foreach (var resumeNO in runner.GetResumeSnapshotNetworkObjects())
            {
                if (resumeNO.TryGetBehaviour<NetworkPositionRotation>(out var posRot))
                {
                    runner.Spawn(resumeNO, position: posRot.ReadPosition(), rotation: posRot.ReadRotation(), onBeforeSpawned: (runner, newNO) =>
                    {
                        newNO.CopyStateFrom(resumeNO);
                        if (resumeNO.TryGetBehaviour<NetworkBehaviour>(out var myCustomNetworkBehaviour))
                        {
                            newNO.GetComponent<NetworkBehaviour>().CopyStateFrom(myCustomNetworkBehaviour);
                        }
                    });
                }
            }
        }

        #region INetworkRunnerCallbacks
        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
        }

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

        byte[] connectionToken;
    }
}