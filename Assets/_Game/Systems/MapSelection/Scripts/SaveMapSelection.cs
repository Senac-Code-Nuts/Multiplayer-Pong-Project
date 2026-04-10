using UnityEngine;

namespace Pong.Systems.MapSelection
{
    public class SaveMapSelection : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
