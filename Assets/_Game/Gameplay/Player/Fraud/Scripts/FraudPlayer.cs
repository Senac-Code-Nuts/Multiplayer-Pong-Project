using UnityEngine;
using Pong.Gameplay.Relics;

namespace Pong.Gameplay.Player
{
    public class FraudPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private bool _isCopyModeActive = false;
        [SerializeField] private int _copyCount = 1;
        [SerializeField] private Relic _relicPrefab;

        public bool IsCopyModeActive => _isCopyModeActive;

        protected override void UseAbility()
        {
            Debug.Log($"Fraud is using ability.");
            ActivateCopyMode();
        }

        private void ActivateCopyMode()
        {
            if (_isCopyModeActive)
                return;

            _isCopyModeActive = true;
            Debug.Log($"{gameObject.name} activated relic copy mode.");
        }

        public void TryCopyRelic(Relic relic)
        {
            if (!_isCopyModeActive)
                return;

            if (relic == null || _relicPrefab == null)
                return;

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

                Instantiate(_relicPrefab, originalRelic.transform.position + spawnOffset, Quaternion.identity);
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