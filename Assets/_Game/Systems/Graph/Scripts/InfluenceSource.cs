using UnityEngine;

namespace Pong.Systems.Graph
{
    public class InfluenceSource : MonoBehaviour
    {
        [field : SerializeField]public float Radius {get; private set;}
        public float weightMultiplier = 2f;
        [SerializeField] private AnimationCurve _fallof;

        public float Evaluate(float distance)
        {
            float normalizedDistance = distance / Radius;

            float curveValue = _fallof.Evaluate(normalizedDistance);

            return Mathf.Lerp(weightMultiplier, 1f, curveValue);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }

}
