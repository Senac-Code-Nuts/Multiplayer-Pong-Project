using Pong.Framework.Strategy;

namespace Pong.Framework.BehaviourTree
{
    public interface INodeStrategy : IStrategy
    {
        /// <summary>
        /// Processa o nó e retorna seus status.
        /// </summary>
        /// <returns> O status do nó </returns>
        Node.Status Process();

        /// <summary>
        /// Reseta o estado do nó para permitir que ele seja processado novamente.
        /// </summary>
        void Reset();
    }
}