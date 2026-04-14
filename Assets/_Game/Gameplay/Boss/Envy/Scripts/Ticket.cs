using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Pong.Gameplay.Boss
{
    public class Ticket : MonoBehaviour
    {
        [PropertyTooltip("Velocity that the slot spins (ex. 500)")]
        [SerializeField] private float _spinVelocity;
        public enum TicketResult
        {
            Yes,
            No
        }

        public TicketResult Result {get; private set;}

        private bool _isTicketRolling;

        public void StartRoll()
        {
            _isTicketRolling = true;
        }

        public void StopRoll()
        {
            _isTicketRolling = false;
            Result = Random.value > 0.5 ? TicketResult.Yes : TicketResult.No;

            UpdateVisual();
        }

        private void Update()
        {
            if(_isTicketRolling)
            {
                transform.Rotate(Vector3.up * _spinVelocity * Time.deltaTime);
            }
        }

        private void UpdateVisual()
        {
            //método para visual dos slots do caça niquel
        }


    }
}
