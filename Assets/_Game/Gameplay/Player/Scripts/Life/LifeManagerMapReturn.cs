using Pong.Systems.MapSelection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pong.Gameplay.Life
{
    public class LifeManagerMapReturn : MonoBehaviour
    {
        [SerializeField] private string _mapSceneName = "Map";

        private void OnEnable()
        {
            LifeManager.OnDieEvent += HandlePartyDeath;
        }

        private void OnDisable()
        {
            LifeManager.OnDieEvent -= HandlePartyDeath;
        }

        private void HandlePartyDeath()
        {
            Debug.Log("[LifeManager] Vida do grupo chegou a 0. Voltando para o mapa.");

            SceneManager.LoadScene(_mapSceneName);
        }
    }
}