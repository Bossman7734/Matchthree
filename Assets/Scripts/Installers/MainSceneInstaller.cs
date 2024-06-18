using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller
{
    [SerializeField] private Camera _camera;
    public override void InstallBindings()
    {
        Container.BindInstance(_camera);
    }
}