using UnityEngine;
using Pong.Gameplay.Enemy.Succubus;

namespace Pong.Gameplay.Boss.Greed
{
    public class GreedBoss : BossActor
    {

        [Header("Addiction Settings")]
        [SerializeField] private float _intervalTime;

        /// <summary>
        /// 1. Usar o intervalStrategy da Sucubbus, pois como descrito no GDD, os movimento são similares, logo não preciso criar um novo.
        /// </summary>

        public override void ExecuteAttack()
        {
            Debug.Log($"{gameObject.name} executed treasure protection behavior.");
        }
    }
}