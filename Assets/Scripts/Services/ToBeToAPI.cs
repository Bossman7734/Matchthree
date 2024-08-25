using Extensions.System;
using UnityEngine;
using Zenject.SpaceFighter;

namespace Services
{
    public class ToBeToAPI
    {
        private static bool _currentGroup;
        private const float ABGGroupChance = 0.5f;
        private const string ABTestPrefKey = "ABTest";
        public static ToBeToAPI Ins{get;private set;}

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void RuntimeInitializeOnload()
        {
            Ins = new ToBeToAPI();
            
            float randomABGroup = Random.value;

            bool ab = randomABGroup > ABGGroupChance;

            _currentGroup = ab;

            if (PlayerPrefs.HasKey(ABTestPrefKey) == false)
            {
                PlayerPrefs.SetInt(ABTestPrefKey, _currentGroup.ToInt());
                
                Debug.LogWarning("AbTest Init");
            }
            else
            {
                _currentGroup = PlayerPrefs.GetInt(ABTestPrefKey).ToBool();
            }
            
            Debug.LogWarning($"AB Group: {_currentGroup.ToInt()}");
        }
        public int GetGroup()
        {
            return _currentGroup.ToInt();
        }
        
        public void ForceSetGroup(bool group)
        {
            PlayerPrefs.SetInt(ABTestPrefKey, group.ToInt());
        }
        

    }
}