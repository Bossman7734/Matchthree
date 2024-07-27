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


  private void Start()
  {
      _musicSlider.value = PlayerPrefs.GetFloat("Volume_Value");
  }

  public void Volume_Value (float volume)
  {
      _audioMixer.SetFloat("volume", volume);
      PlayerPrefs.SetFloat("Volume_Value",_musicSlider.value);
  }
  
}
