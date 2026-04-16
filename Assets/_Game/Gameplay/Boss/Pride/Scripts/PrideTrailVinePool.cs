using Pong.Framework.ObjectPool;

namespace Pong.Gameplay.Boss
{
    public class PrideTrailVinePool : ObjectPool<PrideTrailVine>
    {
        public PrideTrailVinePool(
            PrideTrailVine prefab,
            int initialSize,
            UnityEngine.Transform parent = null
        ) : base(prefab, initialSize, parent)
        {
        }
    }
}