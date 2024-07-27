using System;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using UnityEngine.UI;

public class VibrationToggle : EventListenerMono
{
    [SerializeField] private Toggle _toggle;

    private void Awake()
    {
        _toggle = GameObject.Find("VibrationToggle").GetComponent<Toggle>();

        // Set the toggle state based on saved preferences
        _toggle.isOn = PlayerPrefs.GetInt("Vibration") == 1;

        // Add a listener to handle the toggle change
        _toggle.onValueChanged.AddListener(delegate { saveVibration(); });
    }

    public void saveVibration()
    {
        // Save the toggle state in PlayerPrefs
        PlayerPrefs.SetInt("Vibration", _toggle.isOn ? 1 : 0);
    }

    private void OnDestroy()
    {
        // Remove the listener when the object is destroyed to avoid memory leaks
        if (_toggle != null)
        {
            _toggle.onValueChanged.RemoveAllListeners();
        }
    }

    protected override void RegisterEvents()
    {
        _toggle.onValueChanged.AddListener(delegate { saveVibration(); });
    }

    protected override void UnRegisterEvents()
    {
        _toggle.onValueChanged.RemoveListener(delegate { saveVibration(); });
    }
}