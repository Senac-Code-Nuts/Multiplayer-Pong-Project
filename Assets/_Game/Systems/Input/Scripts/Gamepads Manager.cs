using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

namespace Pong.Systems
{
    public class GamepadsManager : MonoBehaviour
    {
        private Gamepad[] _gamepads = new Gamepad[4] {null,null,null,null};
        private int _currentGamepadsNumber = 0;

        [SerializeField] private InputReader[] _inputReaders = new InputReader[4];

        private void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChange;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added) {

                if (device is Gamepad) {

                    if (_currentGamepadsNumber == _gamepads.Length) { return; }

                    _gamepads[Array.IndexOf(_gamepads,null)] = (Gamepad)device;


                    _currentGamepadsNumber++;

                }

            }

            if (change == InputDeviceChange.Removed) { 
                
                if (device is Gamepad) {

                    if (!_gamepads.Contains(device)) { return; }
                    _gamepads[Array.IndexOf(_gamepads,(Gamepad)device)] = null;
                    _currentGamepadsNumber--;
                }
            
            }
        }
    }
}