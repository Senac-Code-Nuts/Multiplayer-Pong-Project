using UnityEngine;

namespace Pong.Core
{
    public static class GameBootstrapper
    {
        private const string BOOTSTRAP_TAG = "<color=green><b>[Bootstrapper]</b></color>";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            GameObject bootstrapper = Resources.Load<GameObject>("CoreSystems");

            if (bootstrapper == null)
            {
                Debug.LogError($"{BOOTSTRAP_TAG}: Failed to create bootstrapper GameObject.");
                return;
            }

            GameObject instance = Object.Instantiate(bootstrapper);
            instance.name = "[ CORE SYSTEMS ]";
            Object.DontDestroyOnLoad(instance);

            if (GameStateSystem.Instance != null)
            {
                GameStateSystem.Instance.ResetToMenu();
            }

            Debug.Log($"{BOOTSTRAP_TAG}: Core systems initialized successfully.");
        }
    }
}
