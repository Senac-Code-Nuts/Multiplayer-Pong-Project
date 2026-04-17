using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace MenuManager
{
    public class CutsceneController : MonoBehaviour
    {
        [Header("Cutscene")]
        [SerializeField] private GameObject _cutscenePanel;
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private string _nextSceneName = "Map";

        private MenuMusicPlayer _menuMusicPlayer;

        [Header("Skip")]
        [SerializeField, Min(0.1f)] private float _holdToSkipDuration = 1.5f;
        [SerializeField] private Image _skipProgressFillImage;
        [SerializeField] private bool _enableKeyboardForTesting = true;

        private bool _isPlaying = false;
        private bool _isEnding = false;
        private float _currentHoldTime = 0f;

        private void OnEnable()
        {
            _menuMusicPlayer = GetComponent<MenuMusicPlayer>();

            if (_videoPlayer != null)
            {
                _videoPlayer.loopPointReached += OnVideoFinished;
            }
        }

        private void OnDisable()
        {
            if (_videoPlayer != null)
            {
                _videoPlayer.loopPointReached -= OnVideoFinished;
            }
        }

        public void PlayCutscene()
        {
            _isPlaying = true;
            _isEnding = false;
            _currentHoldTime = 0f;

            _menuMusicPlayer?.PauseMusic();

            if (_cutscenePanel != null)
            {
                _cutscenePanel.SetActive(true);
            }

            if (_videoPlayer != null)
            {
                string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, "cutscene/cutscene.mp4");
                _videoPlayer.source = VideoSource.Url;
                _videoPlayer.url = videoPath;
                _videoPlayer.time = 0;
                _videoPlayer.Play();
            }

            UpdateSkipUI();
        }

        private void Update()
        {
            if (!_isPlaying || _isEnding) return;

            bool isCastHeld = IsAnyCastHeld();

            if (isCastHeld)
            {
                _currentHoldTime += Time.deltaTime;

                if (_currentHoldTime >= _holdToSkipDuration)
                {
                    EndCutscene();
                    return;
                }
            }
            else
            {
                _currentHoldTime = 0f;
            }

            UpdateSkipUI();
        }

        private void OnVideoFinished(VideoPlayer vp)
        {
            EndCutscene();
        }

        public void EndCutscene()
        {
            if (_isEnding) return;

            _isEnding = true;
            _isPlaying = false;
            _currentHoldTime = 0f;

            if (_videoPlayer != null && _videoPlayer.isPlaying)
            {
                _videoPlayer.Stop();
            }

            if (_cutscenePanel != null)
            {
                _cutscenePanel.SetActive(false);
            }

            _menuMusicPlayer?.ResumeMusic();

            UpdateSkipUI();
            SceneManager.LoadScene(_nextSceneName);
        }

        private bool IsAnyCastHeld()
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                if (gamepad != null && gamepad.buttonSouth.isPressed)
                {
                    return true;
                }
            }

            if (_enableKeyboardForTesting && Keyboard.current != null)
            {
                if (Keyboard.current.enterKey.isPressed)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateSkipUI()
        {
            if (_skipProgressFillImage == null) return;

            float progress = Mathf.Clamp01(_currentHoldTime / _holdToSkipDuration);
            _skipProgressFillImage.fillAmount = progress;
        }
    }
}