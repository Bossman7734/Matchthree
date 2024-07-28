using System;
using Events;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Components.UI.MainMenü
{
    public class MainMenuManager : EventListenerMono
    {
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _settingsMenuPanel;

        private void Awake()
        {
            // Check if the GameObjects are assigned
            if (_mainMenuPanel == null || _settingsMenuPanel == null)
            {
                Debug.LogError("MainMenuPanel or SettingsMenuPanel is not assigned.");
                return;
            }
            SetPanelActive(_mainMenuPanel);
        }

        private void SetPanelActive(GameObject panel)
        {
            if (_mainMenuPanel != null && _settingsMenuPanel != null)
            {
                _mainMenuPanel.SetActive(_mainMenuPanel == panel);
                _settingsMenuPanel.SetActive(_settingsMenuPanel == panel);
            }
            else
            {
                Debug.LogWarning("One or more panels are missing.");
            }
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
            MainMenüEvents.SettingsBTN -= OnSettingsBTN;
            MainMenüEvents.SettingsExitBTN -= OnSettingsExitBTN;
            MainMenüEvents.NewGameBTN -= OnNewGameBTN;
        }
    }
}