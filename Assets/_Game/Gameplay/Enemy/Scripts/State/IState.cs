using UnityEngine;

namespace Pong.Gameplay.Enemy
{
    public interface IState
    {
        void EnterState(EnemyActor succubus);
        void UpdateState(EnemyActor succubus);
        void ExitState(EnemyActor succubus);
    }
}
