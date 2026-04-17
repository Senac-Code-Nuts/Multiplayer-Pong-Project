using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public interface IBossState
    {
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }
}
