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
            int masterLevel = PlayerPrefs.GetInt("MasterVolume");
            int musicLevel = PlayerPrefs.GetInt("MusicVolume");
            int sfxLevel = PlayerPrefs.GetInt("SFXVolume");
            Debug.Log(masterLevel);
            Debug.Log(musicLevel);
            Debug.Log(sfxLevel);
            MasterSlid.value = masterLevel;
            MusicSlid.value = musicLevel;
            EffectSlid.value = sfxLevel;
        }

        public override void LocalUpdate()
        {

        }

        public void Masterchanged()
        {
            AudioManager.Instance.SetMasterVolume(MasterSlid.value, false);
            PlayerPrefs.SetInt("MasterVolume", (int)(MasterSlid.value));
            PlayerPrefs.Save();
        }

        public void Musichanged()
        {
            AudioManager.Instance.SetMusicVolume(MusicSlid.value, false);
            PlayerPrefs.SetInt("MusicVolume", (int)(MusicSlid.value));
            PlayerPrefs.Save();
        }

        public void EffectsChanged()
        {
            AudioManager.Instance.SetSFXVolume(EffectSlid.value, false);
            PlayerPrefs.SetInt("SFXVolume", (int)(EffectSlid.value));
            PlayerPrefs.Save();
        }
    }
}