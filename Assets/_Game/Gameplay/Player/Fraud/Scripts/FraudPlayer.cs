using UnityEngine;
using Pong.Gameplay.Relics;

namespace Pong.Gameplay.Player
{
    public class FraudPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private bool _isCopyModeActive = false;
        [SerializeField, Range(1, 4)] private int _copyCount = 1;
        [SerializeField] private Relic _relicPrefab;

        public bool IsCopyModeActive => _isCopyModeActive;

        protected override void UseAbility()
        {
            if (_isCopyModeActive) return;

            Debug.Log("Fraud is using ability.");
            ActivateCopyMode();
            StartCoroutine(AbilityCooldownRoutine());
        }

        private void ActivateCopyMode()
        {
            _isCopyModeActive = true;
            Debug.Log($"{gameObject.name} activated relic copy mode.");
        }

        public void TryCopyRelic(Relic relic)
        {
            if (!_isCopyModeActive) return;
            if (relic == null || _relicPrefab == null) return;

            SpawnCopies(relic);
            ConsumeCopyMode();
        }

        private void SpawnCopies(Relic originalRelic)
        {
            for (int i = 0; i < _copyCount; i++)
            {
                Vector3 spawnOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    0f,
                    Random.Range(-0.5f, 0.5f)
                );

                Relic copyRelic = Instantiate(
                    _relicPrefab,
                    originalRelic.transform.position + spawnOffset,
                    Quaternion.identity
                );

                copyRelic.SetAsFraudCopy(true);
            }
        }

        private void ConsumeCopyMode()
        {
            _isCopyModeActive = false;
            Debug.Log($"{gameObject.name} consumed relic copy mode.");
        }

        protected override void OnDamageTaken()
        {
            Debug.Log($"{gameObject.name} took damage.");
        }

        protected override void OnDeath()
        {
            Debug.Log($"{gameObject.name} died.");
        }
    }
}