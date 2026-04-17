using System.Collections;
using System.Collections.Generic;
using TMPro;
using Pong.Systems.Input;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.VFX;
using Pong.Systems.Audio;

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
        [SerializeField] private List<VisualEffect> _countdownSmokeEffects = new List<VisualEffect>();
        [SerializeField, Min(1)] private int _activePriority = 20;
        [SerializeField, Min(0)] private int _inactivePriority = 0;

        [Header("Countdown")]
        [SerializeField, Min(0.05f)] private float _duration = 1f;
        [SerializeField, Min(0f)] private float _cameraFocusLeadTime = 0.2f;

        [Header("AudioClips")]
        [SerializeField] private AudioClip _oneClip;
        [SerializeField] private AudioClip _twoClip;
        [SerializeField] private AudioClip _threeClip;
        [SerializeField] private AudioClip _goClip;

        private readonly List<VisualEffect> _runtimeCountdownSmokeEffects = new List<VisualEffect>();
        private Coroutine _countdownRoutine;
        private Coroutine _goRoutine;
        private int _activeCountdownCameraIndex = -1;

        public bool IsCountdownFinished { get; private set; }
        public bool IsGoFinished { get; private set; }

        private void Awake()
        {
            InitializeCountdownSmokeEffects();
        }

        public void StartMatchCountdown()
        {
            IsCountdownFinished = false;
            IsGoFinished = false;
            _activeCountdownCameraIndex = -1;
            
            _uiGroup.alpha = 1f;
            _gameCamera.Priority = _inactivePriority;
            SetAllCountdownSmokeActive(false);

            if (_countdownRoutine != null) StopCoroutine(_countdownRoutine);
            _countdownRoutine = StartCoroutine(CountdownSequence());
        }

        private IEnumerator CountdownSequence()
        {
            for (int i = 3; i > 0; i--)
            {
                _uiText.text = i.ToString();
                AudioClip clipToPlay = i switch
                {
                    3 => _threeClip,
                    2 => _twoClip,
                    1 => _oneClip,
                    _ => null
                };

                if(clipToPlay != null)
                {
                    AudioManager.Instance.PlaySFX(clipToPlay);
                }

                if (_countdownCameras.Count > 0)
                {
                    int camIndex = (3 - i) % _countdownCameras.Count;
                    SetFocusCamera(camIndex);
                }

                yield return new WaitForSeconds(_duration);
            }

            IsCountdownFinished = true; 

            _uiText.text = "GO!";
            SetFocusCamera(_gameCamera);
            SetAllCountdownSmokeActive(false);

            if (_goRoutine != null) StopCoroutine(_goRoutine);
            _goRoutine = StartCoroutine(FinishGoSequence());
        }

        private IEnumerator FinishGoSequence()
        {
            yield return new WaitForSeconds(1f);
            _uiGroup.alpha = 0f;
            IsGoFinished = true; 
        }

        private void SetFocusCamera(int cameraIndex)
        {
            if (cameraIndex < 0 || cameraIndex >= _countdownCameras.Count)
            {
                return;
            }

            _activeCountdownCameraIndex = cameraIndex;

            foreach (var cam in _countdownCameras)
            {
                cam.Priority = _inactivePriority;
            }

            CinemachineCamera targetCam = _countdownCameras[cameraIndex];
            targetCam.Priority = _activePriority;

            ActivateSmokeForCamera(cameraIndex);
        }

        private void SetFocusCamera(CinemachineCamera targetCam)
        {
            foreach (var cam in _countdownCameras)
            {
                cam.Priority = _inactivePriority;
            }

            targetCam.Priority = _activePriority;
            _activeCountdownCameraIndex = -1;
        }

        private void ActivateSmokeForCamera(int cameraIndex)
        {
            SetAllCountdownSmokeActive(false);

            if (cameraIndex < 0 || cameraIndex >= _runtimeCountdownSmokeEffects.Count)
            {
                return;
            }

            VisualEffect smokeEffect = _runtimeCountdownSmokeEffects[cameraIndex];
            if (smokeEffect == null)
            {
                return;
            }

            smokeEffect.gameObject.SetActive(true);
            smokeEffect.Play();
        }

        private void SetAllCountdownSmokeActive(bool active)
        {
            for (int i = 0; i < _runtimeCountdownSmokeEffects.Count; i++)
            {
                VisualEffect smokeEffect = _runtimeCountdownSmokeEffects[i];
                if (smokeEffect == null)
                {
                    continue;
                }

                if (active)
                {
                    smokeEffect.gameObject.SetActive(true);
                    smokeEffect.Play();
                }
                else
                {
                    smokeEffect.Stop();
                    smokeEffect.gameObject.SetActive(false);
                }
            }
        }

        private void InitializeCountdownSmokeEffects()
        {
            _runtimeCountdownSmokeEffects.Clear();

            for (int i = 0; i < _countdownSmokeEffects.Count; i++)
            {
                VisualEffect smokeEffect = _countdownSmokeEffects[i];
                if (smokeEffect == null)
                {
                    _runtimeCountdownSmokeEffects.Add(null);
                    continue;
                }

                if (smokeEffect.gameObject.scene.IsValid())
                {
                    smokeEffect.gameObject.SetActive(false);
                    _runtimeCountdownSmokeEffects.Add(smokeEffect);
                    continue;
                }

                VisualEffect instance = Instantiate(smokeEffect, transform);
                instance.gameObject.SetActive(false);
                _runtimeCountdownSmokeEffects.Add(instance);
            }
        }
    }
}