using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pong.Systems.Selection
{
    public class CharacterSelectionInputHandler : MonoBehaviour
    {
        private const int MAX_PLAYERS = 4;
        private const float MOVE_THRESHOLD = 0.5f;

        [SerializeField] private CharacterSelectionManager _selectionManager;
        [SerializeField] private bool _enableKeyboardForTesting = false;

        private readonly List<SelectionDeviceData> _registeredDevices = new List<SelectionDeviceData>();

        public void ResetInputState()
        {
            _registeredDevices.Clear();
        }

        private void Update()
        {
            if (_selectionManager == null) return;

            TryRegisterAvailableDevices();
            HandleRegisteredDevicesInput();
        }

        private void TryRegisterAvailableDevices()
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                if (_registeredDevices.Count >= MAX_PLAYERS)
                    return;

                if (IsDeviceRegistered(gamepad))
                    continue;

                if (HasJoinInput(gamepad))
                {
                    RegisterDevice(gamepad);
                }
            }

            if (_enableKeyboardForTesting && Keyboard.current != null)
            {
                if (_registeredDevices.Count >= MAX_PLAYERS)
                    return;

                if (!IsDeviceRegistered(Keyboard.current) && HasJoinInput(Keyboard.current))
                {
                    RegisterDevice(Keyboard.current);
                }
            }
        }

        private void HandleRegisteredDevicesInput()
        {
            for (int i = 0; i < _registeredDevices.Count; i++)
            {
                SelectionDeviceData deviceData = _registeredDevices[i];

                float moveX = ReadMoveX(deviceData.Device);
                bool confirmPressed = ReadConfirmPressed(deviceData.Device);
                bool cancelPressed = ReadCancelPressed(deviceData.Device);

                bool movedRight = moveX > MOVE_THRESHOLD && deviceData.PreviousMoveX <= MOVE_THRESHOLD;
                bool movedLeft = moveX < -MOVE_THRESHOLD && deviceData.PreviousMoveX >= -MOVE_THRESHOLD;

                if (movedRight)
                {
                    _selectionManager.ChangeSelection(deviceData.PlayerIndex, 1);
                }

                if (movedLeft)
                {
                    _selectionManager.ChangeSelection(deviceData.PlayerIndex, -1);
                }

                if (confirmPressed && !deviceData.PreviousConfirmPressed)
                {
                    _selectionManager.ConfirmSelection(deviceData.PlayerIndex);
                }

                if (cancelPressed && !deviceData.PreviousCancelPressed)
                {
                    _selectionManager.CancelSelection(deviceData.PlayerIndex);
                }

                deviceData.PreviousMoveX = moveX;
                deviceData.PreviousConfirmPressed = confirmPressed;
                deviceData.PreviousCancelPressed = cancelPressed;
            }
        }

        private void RegisterDevice(InputDevice device)
        {
            int playerIndex = GetFirstFreePlayerIndex();
            if (playerIndex == -1) return;

            _selectionManager.RegisterPlayer(playerIndex);

            float currentMoveX = ReadMoveX(device);
            bool currentConfirmPressed = ReadConfirmPressed(device);
            bool currentCancelPressed = ReadCancelPressed(device);

            _registeredDevices.Add(new SelectionDeviceData
            {
                Device = device,
                PlayerIndex = playerIndex,
                PreviousMoveX = currentMoveX,
                PreviousConfirmPressed = currentConfirmPressed,
                PreviousCancelPressed = currentCancelPressed
            });
        }

        private int GetFirstFreePlayerIndex()
        {
            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                if (!_selectionManager.IsPlayerRegistered(i))
                    return i;
            }

            return -1;
        }

        private bool IsDeviceRegistered(InputDevice device)
        {
            for (int i = 0; i < _registeredDevices.Count; i++)
            {
                if (_registeredDevices[i].Device == device)
                    return true;
            }

            return false;
        }

        private bool HasJoinInput(InputDevice device)
        {
            return Mathf.Abs(ReadMoveX(device)) > MOVE_THRESHOLD
                || ReadConfirmPressed(device)
                || ReadCancelPressed(device);
        }

        private float ReadMoveX(InputDevice device)
        {
            if (device is Gamepad gamepad)
            {
                Vector2 dpad = gamepad.dpad.ReadValue();
                if (Mathf.Abs(dpad.x) > MOVE_THRESHOLD)
                    return dpad.x;

                return gamepad.leftStick.ReadValue().x;
            }

            if (device is Keyboard keyboard)
            {
                float moveX = 0f;

                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    moveX -= 1f;

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    moveX += 1f;

                return moveX;
            }

            return 0f;
        }

        private bool ReadConfirmPressed(InputDevice device)
        {
            if (device is Gamepad gamepad)
                return gamepad.buttonSouth.isPressed;

            if (device is Keyboard keyboard)
                return keyboard.enterKey.isPressed;

            return false;
        }

        private bool ReadCancelPressed(InputDevice device)
        {
            if (device is Gamepad gamepad)
                return gamepad.buttonEast.isPressed || gamepad.startButton.isPressed;

            if (device is Keyboard keyboard)
                return keyboard.escapeKey.isPressed;

            return false;
        }

        private class SelectionDeviceData
        {
            public InputDevice Device;
            public int PlayerIndex;
            public float PreviousMoveX;
            public bool PreviousConfirmPressed;
            public bool PreviousCancelPressed;
        }
    }
}