using UnityEngine;
using Pong.Gameplay.Relics;

namespace Pong.Gameplay.Player
{
    public class FraudPlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private bool _isCopyModeActive = false;
        private int _copyCount = 1;
        [SerializeField] private Relic _relicPrefab;

        [Header("Debug")]
        [SerializeField] private bool _useDebug;

        public bool IsCopyModeActive => _isCopyModeActive;

        protected override void UseAbility()
        {
            if (_isCopyModeActive) return;

            ActivateCopyMode();
            StartCoroutine(AbilityCooldownRoutine());
        }

        protected override void LevelUp()
        {
            base.LevelUp();
            UpgradeCopyCount();
        }

        private void UpgradeCopyCount()
        {
            _copyCount = _level;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1) && _useDebug)
            {
                LevelUp();
            }

            if(Input.GetKeyDown(KeyCode.F2))
            {
                ActivateCopyMode();
            }

        }
        private void ActivateCopyMode()
        {
            _isCopyModeActive = true;
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
        }

        protected override void OnDamageTaken()
        {

        }

        protected override void OnDeath()
        {

        }
    }
}