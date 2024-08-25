using Installers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(MainSceneSettings), menuName ="Matchtree/" + nameof(MainSceneSettings), order = 0)]
    public class MainSceneSettings : ScriptableObject
    {
       [SerializeField] private MainSceneInstaller.Settings _Settings;
        public MainSceneInstaller.Settings Settings => _Settings;
    }
}