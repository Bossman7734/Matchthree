using Events;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

namespace Components.UI.GameOver
{
    public class MainMenuBTN :EventListenerMono
    {
        [SerializeField] private Button _button;


        protected override void RegisterEvents()
        {
            _button.onClick.AddListener(OnClick); 
        }

        private void OnClick()
        {
            GameOverEvents.MainMenüBTN?.Invoke();
        }


        protected override void UnRegisterEvents()
        {
            _button.onClick.RemoveListener(OnClick); 
        }
    }
}