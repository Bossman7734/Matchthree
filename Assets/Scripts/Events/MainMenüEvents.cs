using UnityEngine.Events;
using Zenject.SpaceFighter;

namespace Events
{
    public static class MainMenüEvents
    {
        public static UnityAction SettingsBTN;
        public static UnityAction ExitMainBTN;
        public static UnityAction SettingsExitBTN;
        public static UnityAction NewGameBTN;
        public static UnityAction<float> SoundValueChanged;
        public static UnityAction<float> MusicValueChanged;
    }
}