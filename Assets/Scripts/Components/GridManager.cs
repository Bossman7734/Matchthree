using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Events;
using Extensions.DoTween;
using Extensions.System;
using Extensions.Unity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Zenject;


namespace Components
{
    public partial class GridManager:  SerializedMonoBehaviour, ITweenContainerBind
    {
        [Inject] private InputEvents InputEvents { get; set; }

        [BoxGroup(Order = 999)] [TableMatrix(SquareCells = true, DrawElementMethod = nameof(DrawTile))]
         [OdinSerialize]
        private Tile[,] _grid;

        [SerializeField] private List<GameObject> _tilePrefabs;


        [SerializeField] private int _gridSizeX;
        [SerializeField] private int _gridSizeY;
        [SerializeField] List<int> _prefabIds;
        [SerializeField] private Bounds _gridBounds;
        [SerializeField] private Transform _transform;


        private Tile _selectedTile;
        private Vector3 _mouseDownPos;
        private Vector3 _mouseUpPos;
        private List<Tile> _currMatchesDebug;
        public ITweenContainer TweenContainer { get; set; }
        private List<MonoPool> _tilePoolsByPrefabID;
        private MonoPool _tilePool0;
        private MonoPool _tilePool1;
        private MonoPool _tilePool2;
        private MonoPool _tilePool3;
        private Tile[,] _tilesToMove;

        private void Awake()
        {
            _tilePoolsByPrefabID = new List<MonoPool>();
            
            for (int preFabID = 0; preFabID < _prefabIds.Count; preFabID++)
            {
                
                MonoPool tilePool = new MonoPool
                (
                    new MonoPoolData
                    (
                        _tilePrefabs[preFabID],
                        10,
                        _transform
                    )
                );
                _tilePoolsByPrefabID.Add(tilePool);
            }
            
            TweenContainer = TweenContain.Install(this);
        }
        


        private void OnEnable()
        {
            RegisterEvents();
        }


        private void OnDisable()
        {
            UnRegisterEvents();
            TweenContainer.Clear();
        }
        private void Start()
        {
            GridEvents.GridLoaded.Invoke(_gridBounds);
        }





        private bool HasMatch(Tile fromTile, Tile toTile, out List<Tile> matches)
        {
            bool hasMatches = false;

            matches = _grid.GetMatchesYAll(toTile);
            matches.AddRange(_grid.GetMatchesXAll(toTile));

            matches.AddRange(_grid.GetMatchesYAll(fromTile));
            matches.AddRange(_grid.GetMatchesXAll(fromTile));

            if (matches.Count > 2) hasMatches = true;

            return hasMatches;
        }

        private bool IsGameOver(out Tile hintTile, out GridDir hintDir)
        {
           Debug.LogWarning("IsGameOver check!");
            hintDir = GridDir.Null;
            hintTile = null;
            
            List<Tile> matches = new();
            
            foreach (Tile fromTile in _grid)
            {
                bool isPearl = false;
                hintTile = fromTile;

                if (hintTile.Coords == new Vector2Int(1, 0))
                {
                    Debug.LogWarning("Break");
                    isPearl = true;
                }
                
                Vector2Int thisCoord = fromTile.Coords;

                Vector2Int topCoord = thisCoord + Vector2Int.up;
                Vector2Int botCoord = thisCoord + Vector2Int.down;
                Vector2Int leftCoord = thisCoord + Vector2Int.left;
                Vector2Int rightCoord = thisCoord + Vector2Int.right;

                if (_grid.IsInsideGrid(topCoord))
                {
                    
                    Tile toTile = _grid.Get(topCoord);
                    _grid.Swap(fromTile,toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile)); 
                    
                    if (isPearl)
                    {
                        Debug.LogWarning($"fromTile.Coords:{fromTile.Coords}, topCoord {topCoord}");
                        Debug.LogWarning($"matches{matches.Count}");
                    }
                    _grid.Swap(toTile, fromTile);
                    
                    if (matches.Count > 0)
                    {  
                        hintDir = GridDir.Up;
                        return false;
                    }
                }
                
                if (_grid.IsInsideGrid(botCoord))
                { 
                    if (isPearl)
                    {
                        Debug.LogWarning(botCoord);
                    }
                    
                    Tile toTile = _grid.Get(botCoord);
                    _grid.Swap(fromTile,toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile)); 
                    
                    _grid.Swap(toTile, fromTile);
                    
                    if (matches.Count > 0)
                    { 
                        hintDir = GridDir.Down;
                        return false;
                    }
                }
                
                if (_grid.IsInsideGrid(leftCoord))
                {  
                    if (isPearl)
                    {
                        Debug.LogWarning(leftCoord);
                    }
                    
                    Tile toTile = _grid.Get(leftCoord);
                    _grid.Swap(fromTile,toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile)); 
                    
                    _grid.Swap(toTile, fromTile);
                    
                    if (matches.Count > 0)
                    {
                        hintDir = GridDir.left;
                        return false;
                    }
                }
                if (_grid.IsInsideGrid(rightCoord))
                {
                    if (isPearl)
                    {
                        Debug.LogWarning(rightCoord);
                    }
                    
                    Tile toTile = _grid.Get(rightCoord);
                    _grid.Swap(fromTile,toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile)); 
                    
                    _grid.Swap(toTile, fromTile);
                    
                    if (matches.Count > 0)
                    {
                        hintDir = GridDir.Right;
                        return false;
                    }
                }
            }
            return matches.Count == 0;
        }

        [Button] //Unit Test 
        private void TestGridDir(Vector2 input)
        {
            Debug.LogWarning(GridF.GetGridDir(input));
        }

        [Button]
        private void TestGameOver()
        {
           bool isGameOver = IsGameOver(out Tile hintTile, out GridDir hintDir);
           
           Debug.LogWarning($"İsGameOver: {isGameOver}, hintTile {hintTile}, hintDir {hintDir}", hintTile);
          
        }

        private void RainDownTiles()
        {
            bool didDestroy = true;
            
            _tilesToMove = new Tile[_gridSizeX, _gridSizeY];


            for (int y = 0; y < _gridSizeY; y++)
            {
                int spawnStartY = 0;
                int spawnQueue = 0;
                for (int x = 0; x < _gridSizeX; x++)
                {
                    Vector2Int thisCoord = new(x, y);
                    Tile thisTile = _grid.Get(thisCoord);

                    if (thisTile) continue;

                    int spawnPoint = _gridSizeY;
                    for (int y1 = y; y1 <= spawnPoint; y1++)
                    {
                        if (y1 == spawnPoint)
                        {
                            if (spawnStartY == 0)
                            {
                                spawnStartY = thisCoord.y;
                            }
                            
                            spawnQueue = thisCoord.y - spawnStartY;
                                
                            MonoPool randomPool = _tilePoolsByPrefabID.Random();
                            Tile newTile = randomPool.Request<Tile>();
                            Vector3 SpawnWorldPos = _grid.CoordsToWorld(_transform,new Vector2Int(x, spawnPoint + spawnQueue));
                            newTile.Teleport(SpawnWorldPos);
                            _grid.Set(newTile, thisCoord);
                            _tilesToMove[thisCoord.x,thisCoord.y] = newTile;
                            break;
                        }

                        Vector2Int emptyCoords = new(x, y1);
                        Tile mostTopTile = _grid.Get(emptyCoords);

                        if (mostTopTile)
                        {
                            _grid.Set(null, mostTopTile.Coords);
                            _grid.Set(mostTopTile, thisCoord);
                            
                            _tilesToMove[thisCoord.x,thisCoord.y] = mostTopTile;
                            break;
                        }
                    }
                }
            }
            

            StartCoroutine(RainDownRoutine());
            
        }

        private IEnumerator RainDownRoutine()
        {
            bool shouldWait = false;
            
            for (int y = 0; y < _gridSizeY; y++)
            {
                for (int x = 0; x < _gridSizeX; x++)
                {
                   Tile thisTile = _tilesToMove[x, y];

                   if (thisTile == false) continue;
                   thisTile.DOMove(_grid.CoordsToWorld(_transform, thisTile.Coords), 1f);
                   yield return new WaitForSeconds(0.1f);

                   shouldWait = true;
                }

                if (shouldWait)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            GridEvents.InputStart?.Invoke();
        }


        private void DotileMoveAnim(Tile fromTile, Tile toTile, TweenCallback onComplate)
        {
            TweenContainer.AddSequence = DOTween.Sequence();
            Vector3 fromTileWorldPos = _grid.CoordsToWorld(_transform, fromTile.Coords);
            TweenContainer.AddedSeq.Append((fromTile.transform.DOMove(fromTileWorldPos, 1f)));
            Vector3 toTileWorldPos = _grid.CoordsToWorld(_transform, toTile.Coords);
            TweenContainer.AddedSeq.Join(toTile.transform.DOMove(toTileWorldPos, 1f));

            TweenContainer.AddedSeq.onComplete += onComplate;
        }

        private void RegisterEvents()
        {
            InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid;
        }
        private void OnMouseDownGrid(Tile clickedTile, Vector3 dirVector)
        {
            _selectedTile = clickedTile;
            _mouseDownPos = dirVector;
        }

        private bool CanMove(Vector2Int tileMoveCoord)
        {
            return _grid.IsInsideGrid(tileMoveCoord);
        }
        
        private void OnMouseUpGrid(Vector3 mouseUpPos)
        {
            _mouseUpPos = mouseUpPos;

            Vector3 dirVector = mouseUpPos - _mouseDownPos;

            if (_selectedTile)
            {
                Vector2Int tileMoveCoord = _selectedTile.Coords + GridF.GetGridDirVector(dirVector);

                if (!CanMove(tileMoveCoord)) return;

                Tile toTile = _grid.Get(tileMoveCoord);

                _grid.Swap(_selectedTile, toTile);

                if (!HasMatch(_selectedTile, toTile, out List<Tile> matches))
                {
                    _grid.Swap(toTile, _selectedTile);
                    return;
                }

                GridEvents.InputStop?.Invoke();

                DotileMoveAnim(_selectedTile, toTile,
                    delegate
                    {
                        matches.DoToAll(e =>
                        {
                            _grid.Set(null, e.Coords);
                            e.gameObject.Destroy();
                        });

                        RainDownTiles();
                    });

                _currMatchesDebug = matches;
            }
        }
        private void UnRegisterEvents()
        {
            InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid;
        }
    }
}
