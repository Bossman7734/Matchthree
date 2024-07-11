using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public class GridEvents
    {
        public static UnityAction<Bounds> GridLoaded;
        public static UnityAction InputStart;
        public static UnityAction InputStop;
    }
}