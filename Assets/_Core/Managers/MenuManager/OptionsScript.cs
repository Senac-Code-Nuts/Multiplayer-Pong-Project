using Pong.Systems.Audio;
using UnityEngine;
using UnityEngine.UI;


namespace MenuManager{
public class OptionsScript : MonoBehaviour
{

    [SerializeField] private Slider MasterSlid;
    [SerializeField] private Slider MusicSlid;
    [SerializeField] private Slider EffectSlid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int masterLevel = PlayerPrefs.GetInt("MasterVolume", 4);
        int musicLevel = PlayerPrefs.GetInt("MusicVolume", 4);
        int sfxLevel = PlayerPrefs.GetInt("SFXVolume", 4);
        MasterSlid.value = masterLevel * 0.25f;
        MusicSlid.value = musicLevel * 0.25f;
        EffectSlid.value = sfxLevel * 0.25f;
    }

    // Update is called once per frame
    
}
}