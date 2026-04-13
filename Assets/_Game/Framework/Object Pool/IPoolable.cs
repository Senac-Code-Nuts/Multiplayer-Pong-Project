namespace Pong.Framework.ObjectPool
{
    public interface IPoolable
    {
        void OnGetFromPool();
        void OnReturnToPool();
    }
}