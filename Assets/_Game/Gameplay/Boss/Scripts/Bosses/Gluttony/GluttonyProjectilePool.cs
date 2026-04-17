using Pong.Framework.ObjectPool;

namespace Pong.Gameplay.Boss
{
    public class GluttonyProjectilePool : ObjectPool<GluttonyProjectile>
    {
        public GluttonyProjectilePool(
            GluttonyProjectile prefab,
            int initialSize,
            UnityEngine.Transform parent = null
        ) : base(prefab, initialSize, parent)
        {
        }
    }
}