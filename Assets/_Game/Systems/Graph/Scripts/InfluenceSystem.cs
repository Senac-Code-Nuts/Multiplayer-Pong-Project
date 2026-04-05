using System.Collections.Generic;
using UnityEngine;

namespace Pong.Systems.Graph
{
    public class InfluenceSystem : MonoBehaviour
    {
        [SerializeField] private List<InfluenceSource> _sources = new List<InfluenceSource>();

        public float GetMultiplier(Vector3 position)
        {
            float total = 0f;

            foreach (var source in _sources)
            {
                float distance = Vector3.Distance(position, source.transform.position);

                if (distance <= source.radius)
                {
                    total += (source.Evaluate(distance) - 1f);
                }
            }

            return 1f + total;
        }
    }
}


