using UnityEngine;

namespace Pong.Gameplay.Player
{
    public class FraudPlayer : PlayerActor
    {
        [Header("Fraud Ability")]
        [SerializeField] private bool _isCopyModeActive = false;

        public bool IsCopyModeActive => _isCopyModeActive;

        public override void UseAbility()
        {
            _isCopyModeActive = true;
            Debug.Log($"{gameObject.name} activated relic copy mode.");
        }

        public void ConsumeCopyMode()
        {
            _isCopyModeActive = false;
            Debug.Log($"{gameObject.name} consumed relic copy mode.");
        }
    }
}