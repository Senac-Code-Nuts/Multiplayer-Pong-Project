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
            float masterLevel = PlayerPrefs.GetFloat("MasterVolume");
            float musicLevel = PlayerPrefs.GetFloat("MusicVolume");
            float sfxLevel = PlayerPrefs.GetFloat("SFXVolume");
            MasterSlid.value = masterLevel;
            MusicSlid.value = musicLevel;
            EffectSlid.value = sfxLevel;
        }

        public override void LocalUpdate()
        {

        }

        public void Masterchanged()
        {
            AudioManager.Instance.SetMasterVolume(MasterSlid.value* 0.25f,false);
            PlayerPrefs.SetFloat("MasterVolume",MasterSlid.value);
        }

        public void Musichanged()
        {
            AudioManager.Instance.SetMusicVolume(MusicSlid.value* 0.25f,false);
            PlayerPrefs.SetFloat("MusicVolume",MusicSlid.value);
        }

        public void EffectsChanged()
        {
            AudioManager.Instance.SetSFXVolume(EffectSlid.value* 0.25f,false);
            PlayerPrefs.SetFloat("SFXVolume",EffectSlid.value);
        }
    }
}