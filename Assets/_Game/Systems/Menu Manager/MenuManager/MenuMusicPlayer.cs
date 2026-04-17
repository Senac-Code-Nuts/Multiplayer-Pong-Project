using Pong.Systems.Audio;
using UnityEngine;

namespace MenuManager
{
    public class MenuMusicPlayer : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private string _musicPath = "Musica/musica_pong_final";
        [SerializeField] private int _channel = 0;
        [SerializeField] private bool _loop = true;
        [SerializeField, Range(0f, 1f)] private float _startingVolume = 0.35f;

        private bool _isPlaying;

        private void Start()
        {
            PlayMusic();
        }

        public void PlayMusic()
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("[MenuMusicPlayer] AudioManager não encontrado na cena.");
                return;
            }

            AudioClip clip = Resources.Load<AudioClip>(_musicPath);
            if (clip == null)
            {
                Debug.LogWarning($"[MenuMusicPlayer] Não foi possível carregar a música no caminho: {_musicPath}");
                return;
            }

            AudioManager.Instance.PlayTrack(clip, _channel, _loop, _startingVolume);
            _isPlaying = true;
        }

        public void PauseMusic()
        {
            if (!_isPlaying || AudioManager.Instance == null)
            {
                return;
            }

            AudioManager.Instance.StopTrack(_channel);
            _isPlaying = false;
        }

        public void ResumeMusic()
        {
            if (_isPlaying)
            {
                return;
            }

            PlayMusic();
        }
    }
}