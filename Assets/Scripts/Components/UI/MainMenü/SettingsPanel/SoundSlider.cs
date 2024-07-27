using System.Collections;
using System.Collections.Generic;
using Events;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : EventListenerMono
{
    [SerializeField] private Slider _slider;


    protected override void RegisterEvents()
    {
        _slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float val)
    {
        MainMenüEvents.SoundValueChanged?.Invoke(val);
    }

    protected override void UnRegisterEvents()
    {
        _slider.onValueChanged.AddListener(OnValueChanged);
    }
}
