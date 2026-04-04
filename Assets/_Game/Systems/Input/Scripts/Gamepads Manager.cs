using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;

namespace Pong.Systems
{
    public class GamepadsManager : MonoBehaviour
    {
        private List<Gamepad> _gamepads = new List<Gamepad>(4);
        private int _currentgamepadsnumber = 0;

        [SerializeField] private InputReader[] _inputreaders = new InputReader[4];

        void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added) {

                if (device is Gamepad) {

                    if (_currentgamepadsnumber == _gamepads.Capacity) { return; }

                    _gamepads.Add((Gamepad)device);  
                    _currentgamepadsnumber++;

                }

            }

            if (change == InputDeviceChange.Removed) { 
                
                if (device is Gamepad) {

                    if (!_gamepads.Contains(device)) { return; }
                    _gamepads[_gamepads.IndexOf((Gamepad)device)] = null;
                    _currentgamepadsnumber--;
                }
            
            }
        }
    }
}