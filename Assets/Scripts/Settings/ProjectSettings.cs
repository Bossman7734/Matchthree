using System;
using Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(ProjectSettings),menuName = EnvVar.ProjectSettingsPath , order = 0)]
    public class ProjectSettings : ScriptableObject
    {
         [SerializeField] private GridManager.Settings gridManagerSettings;
        public GridManager.Settings GridManagerSettings => gridManagerSettings;
    }
}