using UnityEngine;
using Pong.Gameplay.Actors;

namespace Pong.Gameplay.Boss
{
    public abstract class BossActor : Actor
    {
        [Header("Boss")]
        [SerializeField] protected int _phase = 1;

        public int Phase => _phase;

        public abstract void ExecuteAttack();

        protected override void OnDamageTaken()
        {
            Debug.Log($"{gameObject.name} boss took damage.");
        }

        protected override void OnDeath()
        {
            Debug.Log($"{gameObject.name} boss died.");
        }
    }
}