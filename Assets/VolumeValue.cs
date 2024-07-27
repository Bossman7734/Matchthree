using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeValue : MonoBehaviour
{
  [SerializeField] public AudioMixer _audioMixer;
  [SerializeField] public Slider _musicSlider;
  [SerializeField] public Slider _sfxSlider;


  private void Start()
  {
      _musicSlider.value = PlayerPrefs.GetFloat("Volume_Value");
      _sfxSlider.value = PlayerPrefs.GetFloat("SFX_Value");
  }

  public void Volume_Value (float volume)
  {
      _audioMixer.SetFloat("MusicVolume", volume);
      PlayerPrefs.SetFloat("Volume_Value",_musicSlider.value);
  }
  
  public void SFXVolume_Value (float volume)
  {
      _audioMixer.SetFloat("SFXVolume", volume);
      PlayerPrefs.SetFloat("SFX_Value",_sfxSlider.value);
  }
  
}
