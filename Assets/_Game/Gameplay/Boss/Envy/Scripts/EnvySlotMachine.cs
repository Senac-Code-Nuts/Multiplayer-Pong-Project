using UnityEngine;

namespace Pong.Gameplay.Boss
{
    public class EnvySlotMachine : MonoBehaviour
    {
        [SerializeField] private Ticket _ticketA;
        [SerializeField] private Ticket _ticketB;
        [SerializeField] private Ticket _ticketC;

        [SerializeField] private float _rollDuration = 1.5f;

        public bool IsRolling {get;private set;}

        public System.Action<int,int> OnResultReady;


        public void StartRollAll()
        {
            if(IsRolling) return;

            IsRolling = true;
            _ticketA.StartRoll();
            _ticketB.StartRoll();
            _ticketC.StartRoll();
            Invoke(nameof(StopAll), _rollDuration);
        }

        private void StopAll()
        {
            _ticketA.StopRoll();
            _ticketB.StopRoll();
            _ticketC.StopRoll();

            CalculateResult();

            IsRolling = false;
        }

        private void CalculateResult()
        {
            int yesTotal = 0;
            int noTotal = 0;

            Count(_ticketA, ref yesTotal, ref noTotal);
            Count(_ticketB, ref yesTotal, ref noTotal);
            Count(_ticketC, ref yesTotal, ref noTotal);

            IsRolling = false;

            OnResultReady?.Invoke(yesTotal, noTotal);
        }

        private void Count(Ticket ticket, ref int yesTotal, ref int noTotal)
        {
            if(ticket.Result == Ticket.TicketResult.Yes)
            {
                yesTotal++;
            } else
            {
                noTotal++;
            }
        }
    }
}
