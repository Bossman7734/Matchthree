using System;
using Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller> 
    {
        private ProjectEvents _projectEvents;  //Property olan ProjectEvents i introduce/Field a çeviriyoruz...

        public override void InstallBindings()
        {
            _projectEvents = new();      
            Container.BindInstance(_projectEvents).AsSingle();// Singleton gibi fakat değil...
        }

        private void Awake()
        {
            RegisterEvents();
        }

        public override void Start()
        {
            _projectEvents.ProjectStarted?.Invoke();
            
        }

        private void RegisterEvents()  // Class sadece oyun kapandığında deactive olacağından  Unregister etmiyoruz...
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene LoadedScene, LoadSceneMode arg1)
        {
            if (LoadedScene.name == "LoginScene")
            {
                LoadScene("Main");
            }
        }

        private static void LoadScene(string sceneName)  //SceneManager.LoadScene("Main") metodunu Extract Method Yapıyoruz...(LoadScene)
        {
            SceneManager.LoadScene(sceneName); //"Main" kısmını ıntroduce parameters yapıyoruz... 
        }
    }
}
