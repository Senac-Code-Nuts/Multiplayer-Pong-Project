using System;
using MenuManager;
using Pong.Systems.Audio;
using UnityEngine;
using UnityEngine.UI;


namespace MenuManager
{
    public class OptionState : MenuState
    {
        [SerializeField] private Slider MasterSlid;
        [SerializeField] private Slider MusicSlid;
        [SerializeField] private Slider EffectSlid;
        public override void LocalExit()
        {

        }

        public override void LocalFixedUpdate()
        {

        }

        public override void LocalStart()
        {
            SelectThat(_firstButton);
            int masterLevel = PlayerPrefs.GetInt("MasterVolume", 100);
            int musicLevel = PlayerPrefs.GetInt("MusicVolume", 100);
            int sfxLevel = PlayerPrefs.GetInt("SFXVolume", 100);

            MasterSlid.value = masterLevel;
            MusicSlid.value = musicLevel;
            EffectSlid.value = sfxLevel;
        }

        public override void LocalUpdate()
        {

        }

        public void Masterchanged()
        {
            float normalizedVolume = Mathf.Clamp01(MasterSlid.value / 100f);

            AudioManager.Instance.SetMasterVolume(normalizedVolume, false);
            PlayerPrefs.SetInt("MasterVolume", Mathf.RoundToInt(MasterSlid.value));
            PlayerPrefs.Save();
        }

        public void Musichanged()
        {
            float normalizedVolume = Mathf.Clamp01(MusicSlid.value / 100f);

            AudioManager.Instance.SetMusicVolume(normalizedVolume, false);
            PlayerPrefs.SetInt("MusicVolume", Mathf.RoundToInt(MusicSlid.value));
            PlayerPrefs.Save();
        }

        public void EffectsChanged()
        {
            float normalizedVolume = Mathf.Clamp01(EffectSlid.value / 100f);

            AudioManager.Instance.SetSFXVolume(normalizedVolume, false);
            PlayerPrefs.SetInt("SFXVolume", Mathf.RoundToInt(EffectSlid.value));
            PlayerPrefs.Save();
        }
    }
}