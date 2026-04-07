//using Codice.CM.Common;
//using Pong.Gameplay.Relic;
//using UnityEngine;

//namespace Pong.Gameplay.Player
//{
//    public class FraudPlayer : PlayerActor
//    {
//        [Header("Fraud Ability")]
//        [SerializeField] private bool _isCopyModeActive = false;
//        [SerializeField] private int _copyCount = 1;
//        [SerializeField] private Relic _relicPrefab;

//        public override void UseAbility()
//        {
//            _isCopyModeActive = true;
//            Debug.Log($"{gameObject.name} activated relic copy mode.");
//        }

//        public void TryCopyRelic(Relic relic)
//        {
//            if (!_isCopyModeActive)
//                return;

//            if (relic == null || _relicPrefab == null)
//                return;

//            Debug.Log($"{gameObject.name} triggered relic copy.");

//            SpawnCopies(relic);
//            ConsumeCopyMode();
//        }

//        private void SpawnCopies(Relic originalRelic)
//        {
//            //for (int i = 0; i < _copyCount; i++)
//            {
//                Vector3 spawnOffset = new Vector3(
//                    Random.Range(-0.5f, 0.5f),
//                    0f,
//                    Random.Range(-0.5f, 0.5f)
//                );

//                Instantiate(_relicPrefab, originalRelic.transform.position + spawnOffset, Quaternion.identity);
//            }
//        }

//        private void ConsumeCopyMode()
//        {
//            _isCopyModeActive = false;
//            Debug.Log($"{gameObject.name} consumed relic copy mode.");
//        }
//    }
//}