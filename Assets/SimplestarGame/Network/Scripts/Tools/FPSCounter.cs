using UnityEngine;

namespace SimplestarGame
{
    public class FPSCounter : MonoBehaviour
    {
        void Update()
        {
            if (null == NetworkSceneContext.Instance.fpsText)
            {
                return;
            }
            this.deltaTime += (Time.unscaledDeltaTime - this.deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            NetworkSceneContext.Instance.fpsText.text = "FPS: " + fps.ToString("00");
        }

        float deltaTime = 0.0f;
    }
}