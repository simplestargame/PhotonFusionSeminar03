using UnityEngine;
using UnityEngine.UI;

namespace SimplestarGame
{
    public class NetworkSceneContext : MonoBehaviour
    {
        [SerializeField] internal NetworkPlayerInput PlayerInput;
        [SerializeField] internal Text fpsText;
        [SerializeField] internal Text hostClientText;
        [SerializeField] internal Text countText;

        internal static NetworkSceneContext Instance => NetworkSceneContext.instance;

        internal NetworkGame Game;

        void Awake()
        {
            NetworkSceneContext.instance = this;
        }
        static NetworkSceneContext instance;
    }
}