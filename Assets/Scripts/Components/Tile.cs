using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Components
{
    [Serializable]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Vector2Int _coords; 
        [SerializeField] private int ıd;
        public int ID => ıd;
        public Vector2Int Coords => _coords;

        public void Construct(Vector2Int coords)
        {
            _coords = coords;
        }
        
    }
}