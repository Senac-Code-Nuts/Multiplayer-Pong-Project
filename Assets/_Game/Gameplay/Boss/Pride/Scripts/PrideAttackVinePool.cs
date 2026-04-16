using Pong.Framework.ObjectPool;

namespace Pong.Gameplay.Boss
{
    public class PrideAttackVinePool : ObjectPool<PrideAttackVine>
    {
        public PrideAttackVinePool(
            PrideAttackVine prefab,
            int initialSize,
            UnityEngine.Transform parent = null
        ) : base(prefab, initialSize, parent)
        {
        }
    }
}