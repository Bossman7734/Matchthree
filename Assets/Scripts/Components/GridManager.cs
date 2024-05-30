using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components
{
    public class GridManager : SerializedMonoBehaviour
    {
        [BoxGroup(Order = 999)][TableMatrix(SquareCells = true)/*(DrawElementMethod = nameof(DrawTile))*/] [OdinSerialize] 
        private Tile[,] _grid;

        [SerializeField] private List<GameObject> _tilePrefabs;
        

        private int _gridSizeX;
        private int _gridSizeY;

        
        private Tile DrawTile(Rect rect, Tile tile)
        {
            UnityEditor.EditorGUI.DrawRect(rect,Color.blue);

            return tile;
        }
        
         [Button]
        private void CreateGrid(int sizeX, int sizeY)
        {
           _gridSizeX = sizeX;
           _gridSizeY = sizeY;
            
            if (_grid != null)
            {
                foreach (Tile o in _grid)
                {
                  DestroyImmediate(o.gameObject);
                } 
            }

            _grid = new Tile[sizeX, sizeY];

            for (var x = 0; x < _gridSizeX; x++)
            for (var y = 0; y < _gridSizeY; y++)
            {
                Vector2Int coord = new Vector2Int(x,_gridSizeY - y - 1);
                Vector3 pos = new (coord.x,coord.y,0f);
                int randomIndex = Random.Range(0,_tilePrefabs.Count);
                GameObject tilePrefabRandom = _tilePrefabs[randomIndex];

                GameObject tilenew = Instantiate(tilePrefabRandom, pos,quaternion.identity);
                var tile = tilenew.GetComponent<Tile>();
                tile.Construct(coord);
                _grid[x, y] = tile;
                
                
            }

            
        }
    }
}