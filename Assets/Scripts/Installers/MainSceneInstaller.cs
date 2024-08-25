using System;
using System.Collections.Generic;
using Components;
using Services;
using ViewModels;
using Settings;
using UnityEngine;
using ViewModels;
using Zenject;

namespace Installers
{
    public class MainSceneInstaller : MonoInstaller<MainSceneInstaller>
    {
        [Inject] private PlayerVM PlayerVm { get; set; }
        [SerializeField] private Camera _camera;
        private MainSceneSettings _mainSceneSettings;
        private LevelData _currLevel;

        public override void InstallBindings()
        {
            InstallSettings();
            InstallMono();
        }

        private void InstallMono()
        {
            Container.BindInstance(_camera);
        }

        private void InstallSettings()
        {
            _mainSceneSettings = Resources.Load<MainSceneSettings>(EnvVar.SettingsPath + nameof(MainSceneSettings));
        }

        public override void Start()
        {
            GetCurrLevelData();
            PlayerVm.MoveCount = _currLevel.LevelMoveCount;
            InstantiateLevel();
            _mainSceneSettings.Settings.PlayerVm = PlayerVm;
        }

        private void InstantiateLevel()
        {
            Container.InstantiatePrefab(_currLevel.LevelPrefab);
        }

        private void GetCurrLevelData()
        {
            int pLevel = PlayerVm.Level;

            int levelCount = _mainSceneSettings.Settings.Levels.Count;
            pLevel %= levelCount;

            if (ToBeToAPI.Ins.GetGroup() == 0)
            {
                _currLevel = _mainSceneSettings.Settings.Levels[pLevel];
            }
            else
            {
                _currLevel = _mainSceneSettings.Settings.LevelB[pLevel];
            }
        }


        [Serializable]
        public class Settings
        {
            [SerializeField] public PlayerVM PlayerVm;
            [SerializeField] private List<LevelData> _levels;
            [SerializeField] private List<LevelData> _levelsB;
            public List<LevelData> Levels => _levels;
            public List<LevelData> LevelB => _levelsB;

        }
        [Serializable]
        public class LevelData
        {
            [SerializeField] private GameObject _levelPrefab;
            [SerializeField] private int _levelMoveCount;
            public int LevelMoveCount => _levelMoveCount;
            public GameObject LevelPrefab => _levelPrefab ;

        }

    }
}