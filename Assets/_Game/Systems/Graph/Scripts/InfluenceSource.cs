namespace Pong.InfluenceSource
{
    using UnityEngine;
   public class InfluenceSource : MonoBehaviour
{
     public float radius;
     public float weightMultiplier = 2f;
    [SerializeField] private AnimationCurve _fallof;

    public float Evaluate(float distance)
    {
        float t = distance / radius;

        float curveValue = _fallof.Evaluate(t);

        return Mathf.Lerp(weightMultiplier, 1f, curveValue);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
 
}
