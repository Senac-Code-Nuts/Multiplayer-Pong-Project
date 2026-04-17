using UnityEngine;

namespace Pong.Systems.MapSelection
{
    public class MapProgressManager : MonoBehaviour
    {
        public static MapProgressManager Instance { get; private set; }

        private const string UNLOCKED_PHASE_KEY = "UnlockedPhase";

        public int CurrentPhaseInProgress { get; private set; } = -1;
        public int UnlockedPhase { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }

        public void SetCurrentPhase(int phaseIndex)
        {
            CurrentPhaseInProgress = phaseIndex;
        }

        public void CompleteCurrentPhase()
        {
            if (CurrentPhaseInProgress < 0)
                return;

            int nextUnlockedPhase = CurrentPhaseInProgress + 1;

            if (nextUnlockedPhase > UnlockedPhase)
            {
                UnlockedPhase = nextUnlockedPhase;
                SaveProgress();
            }
        }

        public void ResetProgress()
        {
            CurrentPhaseInProgress = -1;
            UnlockedPhase = 0;
            SaveProgress();
        }

        private void LoadProgress()
        {
            UnlockedPhase = PlayerPrefs.GetInt(UNLOCKED_PHASE_KEY, 0);
        }

        private void SaveProgress()
        {
            PlayerPrefs.SetInt(UNLOCKED_PHASE_KEY, UnlockedPhase);
            PlayerPrefs.Save();
        }
    }
}