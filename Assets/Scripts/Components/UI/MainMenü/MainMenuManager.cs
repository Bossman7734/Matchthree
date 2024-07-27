using System;
using Events;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Components.UI.MainMenü
{
    public class MainMenuManager : EventListenerMono
    {
       [SerializeField] private GameObject _mainMenuPanel;
       [SerializeField] private GameObject _settingsMenuPanel;


       private void Awake()
       {
           SetPanelActive(_mainMenuPanel);
       }

       private void SetPanelActive(GameObject panel)
        {
            _mainMenuPanel.SetActive(_mainMenuPanel == panel);
            _settingsMenuPanel.SetActive(_settingsMenuPanel == panel);
        }
       
        protected override void RegisterEvents()
        {
            MainMenüEvents.SettingsBTN += OnSettingsBTN;
            MainMenüEvents.SettingsExitBTN += OnSettingsExitBTN;
            MainMenüEvents.NewGameBTN += OnNewGameBTN;
        }
        

        private void OnNewGameBTN()
        {
            SceneManager.LoadScene("Main");
        }
        

        private void OnSettingsExitBTN()
        {
            SetPanelActive(_mainMenuPanel);
        }

        private void OnSettingsBTN()
        {
            SetPanelActive(_settingsMenuPanel);
        }

        protected override void UnRegisterEvents()
        {
            MainMenüEvents.SettingsBTN += OnSettingsBTN;
            // MainMenüEvents.SettingsExitBTN += OnSettingsExitBTN;
        }
    }
}
