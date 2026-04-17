using System.Collections;
using UnityEngine;
using Pong.Gameplay.Enemy;
using Pong.Gameplay.Boss;

namespace Pong.Gameplay.Player
{
    public class ViolencePlayer : PlayerActor
    {
        [Header("Ability")]
        [SerializeField] private float _stunRadius = 5f;
        [SerializeField] private float _enemyStunDuration = 1f;
        [SerializeField] private float _bossStunDuration = 1f;

        [Header("VFX")]
        [SerializeField] private GameObject _abilityVfxObject;
        [SerializeField, Range(0f, 5f)] private float _abilityVfxDuration = 1f;

        [Header("Debug")]
        [SerializeField] private bool _useDebug;
        private int _maxEnemyTargets = 1;
        private Coroutine _abilityVfxRoutine;

        protected override void Awake()
        {
            base.Awake();

            if (_abilityVfxObject != null)
            {
                _abilityVfxObject.SetActive(false);
            }
        }

        protected override void UseAbility()
        {
            ShowAbilityVfx();
            StunTargets();
            StartCoroutine(AbilityCooldownRoutine());
        }

        protected override void LevelUp()
        {
            base.LevelUp();
            UpgradeEnemyTargets();
        }
        
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1) && _useDebug)
            {
                LevelUp();
            }

        }


        private void UpgradeEnemyTargets()
        {
            _maxEnemyTargets = _level;
        }

        private void StunTargets()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _stunRadius);
            int stunnedEnemies = 0;

            foreach (Collider hit in hits)
            {
                EnemyActor enemy = hit.GetComponentInParent<EnemyActor>();
                if (enemy != null)
                {
                    if (stunnedEnemies >= _maxEnemyTargets)
                        continue;

                    enemy.ApplyStun(_enemyStunDuration);
                    stunnedEnemies++;
                    continue;
                }

                BossActor boss = hit.GetComponentInParent<BossActor>();
                if (boss != null)
                {
                    boss.ApplyStun(_bossStunDuration);
                }
            }
        }

        private void ShowAbilityVfx()
        {
            if (_abilityVfxObject == null)
            {
                return;
            }

            if (_abilityVfxRoutine != null)
            {
                StopCoroutine(_abilityVfxRoutine);
            }

            _abilityVfxObject.SetActive(true);
            _abilityVfxRoutine = StartCoroutine(HideAbilityVfxRoutine());
        }

        private IEnumerator HideAbilityVfxRoutine()
        {
            yield return new WaitForSeconds(_abilityVfxDuration);

            if (_abilityVfxObject != null)
            {
                _abilityVfxObject.SetActive(false);
            }

            _abilityVfxRoutine = null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _stunRadius);
        }

        protected override void OnDamageTaken()
        {

        }

        protected override void OnDeath()
        {

        }
    }
}