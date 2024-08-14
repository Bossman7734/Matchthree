using Extensions.System;
using UnityEngine;
using Zenject.SpaceFighter;

namespace Services
{
    public class ToBeToAPI
    {
        private const float ABGGroupChance = 0.5f;
        private const string ABTestPrefKey = "ABTest";
        public static ToBeToAPI Ins{get;private set;}

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void RuntimeInitializeOnload()
        {
            Ins = new ToBeToAPI();
            
            float randomABGroup = Random.value;

            bool ab = randomABGroup > ABGGroupChance;

            if (PlayerPrefs.HasKey(ABTestPrefKey) == false)
            {
                PlayerPrefs.SetInt(ABTestPrefKey, ab.ToInt());
                
                Debug.LogWarning("AbTest Init");
            }
            else
            {
                ab = PlayerPrefs.GetInt(ABTestPrefKey).ToBool();
            }
            
            Debug.LogWarning($"AB gROUP: {ab.ToInt()}");
        }

    }
}