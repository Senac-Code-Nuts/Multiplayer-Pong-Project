using Pong.Core.Gizmo;
using UnityEngine;

namespace Pong.Systems.Graph
{
    public class InfluenceSource : MonoBehaviour
    {
        [field: SerializeField] public float Radius { get; private set; }
        public float weightMultiplier = 2f;
        [SerializeField] private AnimationCurve _fallof;

        public enum InfluenceType { Repulsive, Attractive }

        [SerializeField] private InfluenceType _type = InfluenceType.Repulsive;


        public float Evaluate(float distance)
        {
            float normalizedDistance = distance / Radius;
            float curveValue = _fallof.Evaluate(normalizedDistance);

            if (_type == InfluenceType.Attractive)
            {
                return Mathf.Lerp(0.5f, 1f, curveValue);
            }
            
            return Mathf.Lerp(weightMultiplier, 1f, curveValue);
        }

        private void OnDrawGizmos()
        {
            // Círculo com fio azul
            GizmoDrawer.DrawCircle(transform.position, Radius, new Color(0, 0, 1, 0.5f), 32);

            // Círculo com falloff gradiente
            GizmoDrawer.DrawCircleWithFalloff(transform.position, Radius, _fallof, Color.blue, Color.green, 64);

            // Ponto no centro
            GizmoDrawer.DrawPoint(transform.position, 0.3f, Color.cyan);
        }
    }

}
