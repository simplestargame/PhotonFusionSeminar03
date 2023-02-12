using Fusion;
using Fusion.Sockets;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SimplestarGame
{
    [DisallowMultipleComponent]
    public class NetworkConnectionManager : MonoBehaviour
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
            return runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.AutoHostOrClient,
                Address = NetAddress.Any(this.serverPort),
                SessionName = this.sessionName,
                SceneManager = sceneManager,
            });
        }
    }
}