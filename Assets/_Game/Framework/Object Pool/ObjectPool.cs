using System.Collections.Generic;
using UnityEngine;  

namespace Pong.Framework.ObjectPool
{
    public abstract class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T prefab;
        private readonly Transform parent;
        private readonly Queue<T> pool;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;
            pool = new Queue<T>(initialSize);
            for (int i = 0; i < initialSize; i++)
            {
                T obj = Object.Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            if (pool.Count > 0)
            {
                T obj = pool.Dequeue();
                obj.gameObject.SetActive(true);
                if (obj is IPoolable poolable)
                {
                    poolable.OnGetFromPool();
                }
                return obj;
            }
            else
            {
                Debug.LogWarning("Object Pool is empty and expansion is not allowed.");
                return null;
            }
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            if (obj is IPoolable poolable)
            {
                poolable.OnReturnToPool();
            }
            pool.Enqueue(obj);
        }
    }
}
