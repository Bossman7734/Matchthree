using System.Collections;
using System.Collections.Generic;
using Components.UI.GameOver;
using Components.UI.UIButton_Sliders;
using Events;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;


public class TryAgainBTN :EventListenerMono
{
    [SerializeField] private Button _button;


    protected override void RegisterEvents()
    {
        _button.onClick.AddListener(OnClick); 
    }

    private void OnClick()
    {
        GameOverEvents.TryAgainBTN?.Invoke();
    }


    protected override void UnRegisterEvents()
    {
        _button.onClick.RemoveListener(OnClick); 
    }
}
