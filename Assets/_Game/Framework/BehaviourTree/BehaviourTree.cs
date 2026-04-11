namespace Pong.Framework.BehaviourTree
{
    public class BehaviourTree : Node
    {
        public BehaviourTree(string name = "BehaviourTree") : base(name) { }

        /// <summary>
        /// Processa a árvore de comportamento percorrendo seus nós. A travessia é feita em profundidade, começando pelo nó raiz e descendo para seus filhos até atingir um nó folha. Se um nó retorna Success, a travessia continua para o próximo filho; se retorna Failure ou Running, a travessia para e retorna esse status imediatamente.
        /// </summary>
        /// <returns> O status da árvore de comportamento </returns>
        public override Status Process()
        {
            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process();
                if (status != Status.Success) return status;
                currentChild++;
            }
            return Status.Success;
        }
    }
}