using System.Collections;
using Pong.Gameplay.Player;
using UnityEngine;
using UnityEngine.VFX;

namespace Pong.Gameplay.Boss
{
    public class GluttonyConeAttack : MonoBehaviour
    {
        private Vector3 _lockedDirection;
        private Coroutine _vfxRoutine;

        public void BeginTelegraph(Vector3 direction, GameObject coneTelegraph)
        {
            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = Vector3.forward;
            }

            _lockedDirection = direction.normalized;

            if (coneTelegraph == null)
                return;

            coneTelegraph.SetActive(true);
            coneTelegraph.transform.forward = _lockedDirection;
        }

        public void SetDirection(Vector3 direction)
        {
            if (direction.sqrMagnitude <= 0.0001f)
                return;

            _lockedDirection = direction.normalized;
        }

        public void UpdateTelegraph(
            float progress,
            float coneRadius,
            float coneAngle,
            GameObject coneTelegraph
        )
        {
            if (coneTelegraph == null)
                return;

            float radius = Mathf.Lerp(0.1f, coneRadius, progress);
            float width = Mathf.Tan(coneAngle * 0.5f * Mathf.Deg2Rad) * radius;

            coneTelegraph.transform.forward = _lockedDirection;
            coneTelegraph.transform.localScale = new Vector3(width, 1f, radius);
        }

        public void EndTelegraph(GameObject coneTelegraph)
        {
            if (coneTelegraph != null)
            {
                coneTelegraph.SetActive(false);
            }
        }

        public void Execute(
            Transform origin,
            int damage,
            float coneRadius,
            float coneAngle,
            LayerMask playerLayerMask,
            GameObject coneTelegraph,
            VisualEffect drinkVfx,
            Transform drinkVfxSpawnPoint,
            float drinkVfxStopDelay
        )
        {
            if (coneTelegraph != null)
            {
                coneTelegraph.SetActive(false);
            }

            PlayVfx(origin, drinkVfx, drinkVfxSpawnPoint, drinkVfxStopDelay, coneRadius);
            ApplyDamage(origin.position, damage, coneRadius, coneAngle, playerLayerMask);
        }

        private void ApplyDamage(
            Vector3 origin,
            int damage,
            float coneRadius,
            float coneAngle,
            LayerMask playerLayerMask
        )
        {
            Collider[] hits = Physics.OverlapSphere(origin, coneRadius, playerLayerMask);

            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent<PlayerActor>(out PlayerActor player))
                    continue;

                Vector3 directionToTarget = player.transform.position - origin;
                directionToTarget.y = 0f;

                if (directionToTarget.sqrMagnitude <= 0.0001f)
                    continue;

                directionToTarget.Normalize();

                float angle = Vector3.Angle(_lockedDirection, directionToTarget);

                if (angle <= coneAngle * 0.5f)
                {
                    Debug.Log($"<color=red>[Gluttony] Bebida acertou: {player.name}</color>");
                    player.ApplyDamage(damage);
                }
            }
        }

        private void PlayVfx(
            Transform origin,
            VisualEffect vfx,
            Transform vfxSpawnPoint,
            float stopDelay,
            float coneRadius
        )
        {
            if (vfx == null)
                return;

            Transform spawn = vfxSpawnPoint != null ? vfxSpawnPoint : origin;

            vfx.transform.SetPositionAndRotation(
                spawn.position,
                Quaternion.LookRotation(_lockedDirection, Vector3.up)
            );

            if (vfx.HasFloat("SpitLength"))
            {
                vfx.SetFloat("SpitLength", coneRadius);
            }

            vfx.Stop();
            vfx.Reinit();
            vfx.SendEvent("DrinkSpit");

            if (_vfxRoutine != null)
            {
                StopCoroutine(_vfxRoutine);
            }

            _vfxRoutine = StartCoroutine(StopVfxRoutine(vfx, stopDelay));
        }

        private IEnumerator StopVfxRoutine(VisualEffect vfx, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (vfx != null)
            {
                vfx.Stop();
            }

            _vfxRoutine = null;
        }
    }
}