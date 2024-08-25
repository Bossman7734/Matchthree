using System.Numerics;
using Components;
using UnityEngine.Events;

namespace Events
{
    public class ProjectEvents
    {
        public UnityAction ProjectStarted;
        public UnityAction<Tile, Vector3> MouseDownGrid;
        public UnityAction<Vector3> MouseUpGrid;
        public static UnityAction LevelComplete;
    }
}