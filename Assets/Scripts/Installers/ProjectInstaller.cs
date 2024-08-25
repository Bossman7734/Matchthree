using System;
using Components;

using Events;
using Settings;
using TMPro;
using Extensions.Unity;
using ViewModels;
using Unity.Android.Types;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using ViewModels;
using Zenject;
using Zenject.Internal;
using static Events.MainMenüEvents;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller> 
    {
        
        private ProjectEvents _projectEvents;  //Property olan ProjectEvents i introduce/Field a çeviriyoruz...
        private  InputEvents _InputEvents;
        private GridEvents _gridEvents;
        private ProjectSettings _projectSettings;
        
        public override void InstallBindings()
        {
            InstallEvents();
            InstallSettings();
            InstallData();
            
        }

        private void InstallData()
        {
            Container.BindInterfacesAndSelfTo<PlayerVM>().AsSingle();
        }


        private void InstallSettings()
        {
            _projectSettings = Resources.Load<ProjectSettings>(EnvVar.ProjectSettingsPath);
            Container.BindInstance(_projectSettings).AsSingle();
        }

        private void InstallEvents()
        {
            _projectEvents = new();      
            Container.BindInstance(_projectEvents).AsSingle();// Singleton gibi fakat değil...
            
            _InputEvents = new InputEvents();
            Container.BindInstance(_InputEvents).AsSingle();

            _gridEvents = new GridEvents();
            Container.BindInstance(_gridEvents).AsSingle();
        }

        private void Awake() { RegisterEvents(); }

        public override void Start()
        {
            _projectEvents.ProjectStarted?.Invoke();
           
            if ( SceneManager . GetActiveScene () . name == EnvVar . LoginSceneName )
            {
                LoadScene ( EnvVar . MainSceneName ) ;
            }
            
        }
        private static void LoadScene(string sceneName)  //SceneManager.LoadScene("Main") metodunu Extract Method Yapıyoruz...(LoadScene)
        {
            SceneManager.LoadScene(sceneName); //"Main" kısmını ıntroduce parameters yapıyoruz... 
        }


        private void RegisterEvents()  // Class sadece oyun kapandığında deactive olacağından  Unregister etmiyoruz...
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            ProjectEvents.LevelComplete += OnLevelComplete;
        }

        private void OnLevelComplete()
        {
            LoadScene(EnvVar.MainSceneName);
        }


        private void OnSceneLoaded(Scene LoadedScene, LoadSceneMode arg1)
        {
            // if (LoadedScene.name == EnvVar.LoginSceneName)
            // {
            //    SceneManager.LoadScene(EnvVar.MainSceneName);
            // }
        }
    }
}
