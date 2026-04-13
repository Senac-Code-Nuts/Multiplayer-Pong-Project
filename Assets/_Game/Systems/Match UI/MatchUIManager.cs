using System.Collections;
using System.Collections.Generic;
using TMPro;
using Pong.Systems.Input;
using UnityEngine;
using Unity.Cinemachine;

namespace Pong.Shared.Management
{
    public class MatchUIManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private CanvasGroup _uiGroup;
        [SerializeField] private TMP_Text _uiText;

        [Header("Cameras")]
        [SerializeField] private CinemachineCamera _gameCamera;
        [SerializeField] private List<CinemachineCamera> _countdownCameras = new List<CinemachineCamera>();
        [SerializeField, Min(1)] private int _activePriority = 20;
        [SerializeField, Min(0)] private int _inactivePriority = 0;

        [Header("Countdown")]
        [SerializeField, Min(0.05f)] private float _duration = 1f;
        [SerializeField, Min(0f)] private float _cameraFocusLeadTime = 0.2f;

        private Coroutine _countdownRoutine;
        private Coroutine _goRoutine;

        public bool IsCountdownFinished { get; private set; }
        public bool IsGoFinished { get; private set; }

        private void Awake()
        {
            HideGroup(_uiGroup);
            ClearText(_uiText);
            IsCountdownFinished = false;
            SetGameCameraActive();
        }

        public void StartCountdown()
        {
            if (_countdownRoutine != null)
            {
                StopCoroutine(_countdownRoutine);
            }

            if (_goRoutine != null)
            {
                StopCoroutine(_goRoutine);
                _goRoutine = null;
            }

            IsGoFinished = false;

            _countdownRoutine = StartCoroutine(CountdownRoutine());
        }

        public void ShowGo()
        {
            if (_goRoutine != null)
            {
                StopCoroutine(_goRoutine);
            }

            _goRoutine = StartCoroutine(GoRoutine());
        }

        public void PlayGoAnimation()
        {
            ShowGo();
        }

        private IEnumerator CountdownRoutine()
        {
            IsCountdownFinished = false;
            ShowGroup(_uiGroup);

            int playersToReveal = Mathf.Min(GetPlayerCountFromGamepads(), _countdownCameras.Count);

            for (int i = 0; i < playersToReveal; i++)
            {
                SetCountdownCameraActive(i);
                if (_cameraFocusLeadTime > 0f)
                {
                    yield return new WaitForSeconds(_cameraFocusLeadTime);
                }

                if (GamepadsManager.Instance != null)
                {
                    yield return GamepadsManager.Instance.RevealPlayerRoutine(i);
                }

                SetText(_uiText, (playersToReveal - i).ToString());
                yield return new WaitForSeconds(_duration);

                if (i < playersToReveal - 1)
                {
                    ClearText(_uiText);
                }
            }

            ClearText(_uiText);
            HideGroup(_uiGroup);
            SetGameCameraActive();
            IsCountdownFinished = true;
            _countdownRoutine = null;
        }

        private IEnumerator GoRoutine()
        {
            IsGoFinished = false;
            SetGameCameraActive();
            ShowGroup(_uiGroup);
            SetText(_uiText, "GO!");

            yield return new WaitForSeconds(_duration);

            ClearText(_uiText);
            HideGroup(_uiGroup);
            IsGoFinished = true;
            _goRoutine = null;
        }

        private void SetCountdownCameraActive(int countdownIndex)
        {
            if (_countdownCameras == null || _countdownCameras.Count == 0)
            {
                SetGameCameraActive();
                return;
            }

            for (int i = 0; i < _countdownCameras.Count; i++)
            {
                CinemachineCamera camera = _countdownCameras[i];
                if (camera == null)
                {
                    continue;
                }

                if (i < countdownIndex - 1)
                {
                    camera.Priority.Value = _inactivePriority;
                }
            }

            int cameraIndex = countdownIndex % _countdownCameras.Count;
            CinemachineCamera activeCamera = _countdownCameras[cameraIndex];

            if (activeCamera != null)
            {
                activeCamera.Priority.Value = _activePriority;
            }

            int previousCameraIndex = countdownIndex - 1;
            if (previousCameraIndex >= 0 && previousCameraIndex < _countdownCameras.Count)
            {
                CinemachineCamera previousCamera = _countdownCameras[previousCameraIndex];
                if (previousCamera != null)
                {
                    previousCamera.Priority.Value = _inactivePriority;
                }
            }

            if (_gameCamera != null)
            {
                _gameCamera.Priority.Value = _inactivePriority;
            }
        }

        private void SetGameCameraActive()
        {
            if (_countdownCameras != null)
            {
                for (int i = 0; i < _countdownCameras.Count; i++)
                {
                    CinemachineCamera camera = _countdownCameras[i];
                    if (camera == null)
                    {
                        continue;
                    }

                    camera.Priority.Value = _inactivePriority;
                }
            }

            if (_gameCamera != null)
            {
                _gameCamera.Priority.Value = _activePriority;
            }
        }

        private void ShowGroup(CanvasGroup canvasGroup)
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private void HideGroup(CanvasGroup canvasGroup)
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void SetText(TMP_Text text, string value)
        {
            if (text == null)
            {
                return;
            }

            text.text = value;
        }

        private void ClearText(TMP_Text text)
        {
            SetText(text, string.Empty);
        }

        private int GetPlayerCountFromGamepads()
        {
            if (GamepadsManager.Instance == null)
            {
                return 2;
            }

            return Mathf.Clamp(GamepadsManager.Instance.GetPlayerCount(), 2, 4);
        }
    }
}