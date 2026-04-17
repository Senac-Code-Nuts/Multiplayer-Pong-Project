using UnityEngine;

namespace Pong.Systems.MapSelection
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MapButton[] _mapButtons;
        [SerializeField] private GameObject[] _connections;
        [SerializeField] private bool _hideLockedButtons = true;

        private const string PHASE_KEY = "NewPhase";

        private void Start()
        {
            RefreshMap();
        }

        public void RefreshMap()
        {
            int currentPhase = 0;

            if (MapProgressManager.Instance != null)
            {
                currentPhase = MapProgressManager.Instance.UnlockedPhase;
            }

            for (int i = 0; i < _mapButtons.Length; i++)
            {
                if (_mapButtons[i] == null)
                    continue;

                if (i < currentPhase)
                {
                    _mapButtons[i].SetCompleted();
                }
                else if (i == currentPhase)
                {
                    _mapButtons[i].SetUnlocked();
                }
                else
                {
                    _mapButtons[i].SetLocked(_hideLockedButtons);
                }
            }

            for (int i = 0; i < _connections.Length; i++)
            {
                if (_connections[i] == null)
                    continue;

                bool shouldShowConnection = i < currentPhase;
                _connections[i].SetActive(shouldShowConnection);
            }
        }

        public void ResetProgress()
        {
            if (MapProgressManager.Instance != null)
            {
                MapProgressManager.Instance.ResetProgress();
            }

            RefreshMap();
        }
    }
}