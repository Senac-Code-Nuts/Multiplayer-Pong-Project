using UnityEngine;

namespace Pong.Core.Gizmo
{
    public static class GizmoDrawer
    {
        public static void DrawCircle(Vector3 position, float radius, Color color, int segments = 32)
        {
            Gizmos.color = color;
            DrawCircleGizmo(position, radius, segments);
        }

        public static void DrawCircleGradient(Vector3 position, float radius, Color startColor, Color endColor, int segments = 64)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = position + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 newPoint = position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                float normalizedDistance = (float)i / segments;
                Gizmos.color = Color.Lerp(startColor, endColor, normalizedDistance);
                Gizmos.DrawLine(prevPoint, newPoint);

                prevPoint = newPoint;
            }
        }

        public static void DrawCircleWithFalloff(Vector3 position, float radius, AnimationCurve falloffCurve,
            Color centerColor, Color edgeColor, int segments = 64)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = position + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 newPoint = position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                float normalizedDistance = (float)i / segments;
                float curveValue = falloffCurve.Evaluate(normalizedDistance);

                Gizmos.color = Color.Lerp(centerColor, edgeColor, curveValue);
                Gizmos.DrawLine(prevPoint, newPoint);

                prevPoint = newPoint;
            }
        }

        public static void DrawChargingCircle(Vector3 position, float currentRadius, float maxRadius,
            Color progressColor, Color maxRadiusColor, int segments = 32)
        {
            // Círculo em progresso
            Gizmos.color = progressColor;
            DrawCircleGizmo(position, currentRadius, segments);

            // Círculo máximo em semi-transparente
            Gizmos.color = new Color(maxRadiusColor.r, maxRadiusColor.g, maxRadiusColor.b, 0.3f);
            DrawCircleGizmo(position, maxRadius, segments);
        }

        public static void DrawPoint(Vector3 position, float size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(position, size);
        }

        private static void DrawCircleGizmo(Vector3 position, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = position + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                Vector3 newPoint = position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }
    }
}
