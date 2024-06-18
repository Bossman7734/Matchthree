using System.Collections.Generic;
using DefaultNamespace;
using Events;
using Extensions.System;
using Extensions.Unity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Components
{
    public class GridManager : SerializedMonoBehaviour
    {
        [BoxGroup(Order = 999)][TableMatrix(SquareCells = true)/*(DrawElementMethod = nameof(DrawTile))*/] 
        [OdinSerialize] private Tile[,] _grid;

        [SerializeField] private List<GameObject> _tilePrefabs;
        

        private int _gridSizeX;
        private int _gridSizeY;
        [SerializeField] List<int> _prefabIds;

        private Tile _selectedTile;
        private Vector3 _mouseDownPos;
        private Vector3 _mouseUpPos;


        private Tile DrawTile(Rect rect, Tile tile)
        {
            UnityEditor.EditorGUI.DrawRect(rect,Color.blue);

            return tile;
        }
        
         [Button]
        private void CreateGrid(int sizeX, int sizeY)
        {
            _prefabIds = new();
            for (int id = 0; id < _tilePrefabs.Count; id++) _prefabIds.Add(id);
            
            
           _gridSizeX = sizeX;
           _gridSizeY = sizeY;
            
            if (_grid != null)
            {
                foreach (Tile o in _grid)
                {
                  DestroyImmediate(o.gameObject);
                } 
            }

            _grid = new Tile[_gridSizeX,_gridSizeY];

            for (int x = 0; x < _gridSizeX; x++)
            for (int y = 0; y < _gridSizeY; y++)
            {
                List<int> spawnableIds = new(_prefabIds);
                Vector2Int coord = new(x,_gridSizeY - y - 1); // Invert Y Axis
                Vector3 pos = new (coord.x,coord.y,0f);
                
                _grid.GetSpawnableColors( coord, spawnableIds);

                
                
                int randomId = spawnableIds.Random();
                
                
                GameObject tilePrefabRandom = _tilePrefabs[randomId];
                GameObject tilenew = Instantiate(tilePrefabRandom, pos,Quaternion.identity); //Instantiate random prefabs
                
                Tile tile = tilenew.GetComponent<Tile>();
                tile.Construct(coord);
                
                _grid[coord.x, coord.y ] = tile;
                
                
            }
        }
        private void OnEnable()
        {
            RegisterEvents();
        }

        private void OnDisable()
        {
            UnRegisterEvents();
        }

        [Inject] private InputEvents InputEvents {get;set;}
        private void RegisterEvents()
        {
            InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid;
        }


        private void OnMouseDownGrid(Tile arg0, Vector3 arg1)
        {
            _selectedTile = arg0;
            _mouseDownPos = arg1;
            EDebug.Method();
        }
        private void OnMouseUpGrid(Vector3 arg0)
        {
            _mouseUpPos = arg0;
              
            if (_selectedTile)
            {
                EDebug.Method();
                Debug.DrawLine(_mouseDownPos, _mouseUpPos, Color.blue, 2f);
            }
        }

        private void UnRegisterEvents()
        {
            InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid; 
        }
    }
}