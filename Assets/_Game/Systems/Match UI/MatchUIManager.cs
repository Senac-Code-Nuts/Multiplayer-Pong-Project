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

        public void StartMatchCountdown()
        {
            IsCountdownFinished = false;
            IsGoFinished = false;
            
            _uiGroup.alpha = 1f;
            _gameCamera.Priority = _inactivePriority;

            if (_countdownRoutine != null) StopCoroutine(_countdownRoutine);
            _countdownRoutine = StartCoroutine(CountdownSequence());
        }

        private IEnumerator CountdownSequence()
        {
            for (int i = 3; i > 0; i--)
            {
                _uiText.text = i.ToString();
                
                if (_countdownCameras.Count > 0)
                {
                    int camIndex = (3 - i) % _countdownCameras.Count;
                    SetFocusCamera(_countdownCameras[camIndex]);
                }

                yield return new WaitForSeconds(_duration);
            }

            IsCountdownFinished = true; 

            _uiText.text = "GO!";
            SetFocusCamera(_gameCamera);

            if (_goRoutine != null) StopCoroutine(_goRoutine);
            _goRoutine = StartCoroutine(FinishGoSequence());
        }

        private IEnumerator FinishGoSequence()
        {
            yield return new WaitForSeconds(1f);
            _uiGroup.alpha = 0f;
            IsGoFinished = true; 
        }

        private void SetFocusCamera(CinemachineCamera targetCam)
        {
            foreach (var cam in _countdownCameras)
            {
                cam.Priority = _inactivePriority;
            }
            targetCam.Priority = _activePriority;
        }
    }
}