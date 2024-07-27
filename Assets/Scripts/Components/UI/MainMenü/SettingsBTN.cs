using System.Collections;
using System.Collections.Generic;
using Components.UI.UIButton_Sliders;
using Events;
using UnityEngine;

public class SettingsBTN : UIButton
{
    protected override void OnClick()
    {
        MainMenüEvents.SettingsBTN?.Invoke();
    }
}
