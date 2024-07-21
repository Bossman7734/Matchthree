using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Events;
using Extensions.DoTween;
using Extensions.System;
using Extensions.Unity;
using ModestTree;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;


namespace Components
{
    public partial class GridManager : SerializedMonoBehaviour, ITweenContainerBind
    {
        [Inject] private InputEvents InputEvents { get; set; }
        [Inject] private GridEvents GridEvents { get; set; }

        [BoxGroup(Order = 999)]
#if  UNITY_EDITOR
        
        [TableMatrix(SquareCells = true, DrawElementMethod = nameof(DrawTile))] 
        
#endif
        [OdinSerialize]
        private Tile[,] _grid;
        [SerializeField] private List<GameObject> _tilePrefabs;
        [SerializeField] private int _gridSizeX;
        [SerializeField] private int _gridSizeY;
        [SerializeField] List<int> _prefabIds;
        [SerializeField] private Bounds _gridBounds;
        [SerializeField] private Transform _transform;
        [SerializeField] private List<GameObject> _tileBGs = new();
        [SerializeField] private List<GameObject> _gridBorders = new();
        [SerializeField] private GameObject _tileBGPrefab;
        [SerializeField] private Transform _bGTrans;
        [SerializeField] private GameObject _borderTopLeft;
        [SerializeField] private GameObject _borderTopRight;
        [SerializeField] private GameObject _borderBotLeft;
        [SerializeField] private GameObject _borderBotRıght;
        [SerializeField] private GameObject _borderLeft;
        [SerializeField] private GameObject _borderRight;
        [SerializeField] private GameObject _borderTop;
        [SerializeField] private GameObject _borderBot;
        [SerializeField] private Transform _borderTrans; 
        [SerializeField] private int _scoreMulti;
        

        private Tile _selectedTile;
        private Vector3 _mouseDownPos;
        private Vector3 _mouseUpPos;
        public ITweenContainer TweenContainer { get; set; }
        private List<MonoPool> _tilePoolsByPrefabID;
        private MonoPool _tilePool0;
        private MonoPool _tilePool1;
        private MonoPool _tilePool2;
        private MonoPool _tilePool3;
        private Tile[,] _tilesToMove;
        [OdinSerialize]private List<List<Tile>> _lastMatches;
        private Tile _hintTile;
        private GridDir _hintDir;
        private Sequence _hintTween;
        private Coroutine _destroyRoutine;
        private Coroutine _hintRoutine;


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
            for (int x = 0; x < _grid.GetLength(0); x++)
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                Tile tile = _grid[x, y];

                SpawnTile(tile.ID, _grid.CoordsToWorld(_transform,
                    tile.Coords), tile.Coords);
                tile.gameObject.Destroy();
            }

            IsGameOver(out _hintTile, out _hintDir);
            GridEvents.GridLoaded?.Invoke(_gridBounds);
            GridEvents.InputStart?.Invoke();
        }

        private bool CanMove(Vector2Int tileMoveCoord)
        {
            return _grid.IsInsideGrid(tileMoveCoord);
        }


        /*private bool HasMatch(Tile fromTile, Tile toTile, out List<List<Tile>> matches)
        {
            matches = new List<List<Tile>>();
            bool hasMatches = false;

            List<Tile> matchesAll = _grid.GetMatchesYAll(toTile);
            matchesAll.AddRange(_grid.GetMatchesXAll(toTile));

            if (matchesAll.Count > 0)
            {
                matches.Add(matchesAll);
            }
            matchesAll = _grid.GetMatchesYAll(fromTile);
            matchesAll.AddRange(_grid.GetMatchesXAll(fromTile));

            if (matchesAll.Count > 0)
            {
                matches.Add(matchesAll);
            }
            
            if (matches.Count > 0) hasMatches = true;

          // matches = matches.Where(e => e.Count > 0).ToList(); // filter example...

            return hasMatches;
        }*/

        private bool HasAnyMatches(out List<List<Tile>> matches)  //we have many list and we take thıs all List adding to one list...
        {
            matches = new List<List<Tile>>();

            foreach (Tile tile in _grid)
            {
                List<Tile> matchesAll = _grid.GetMatchesXAll(tile);
                matchesAll.AddRange(_grid.GetMatchesYAll(tile));

                if (matchesAll.Count > 0)
                {
                    matches.Add(matchesAll);
                }
            }

            matches = matches.OrderByDescending(e => e.Count).ToList();

            for (var i = 0; i < matches.Count; i++)
            {
                List<Tile> match = matches[i];
                
                match = match.Where(e => e.ToBeDestroyed == false).DoToAll(e => e.ToBeDestroyed = true).ToList();
                
            }

            matches = matches.Where(e => e.Count > 2).ToList();

            return matches.Count > 0;
        }

        private bool IsGameOver(out Tile hintTile, out GridDir hintDir)
        {
            hintDir = GridDir.Null;
            hintTile = null;

            List<Tile> matches = new();

            foreach (Tile fromTile in _grid)
            {
                hintTile = fromTile;

                Vector2Int thisCoord = fromTile.Coords;

                Vector2Int topCoord = thisCoord + Vector2Int.up;
                Vector2Int botCoord = thisCoord + Vector2Int.down;
                Vector2Int leftCoord = thisCoord + Vector2Int.left;
                Vector2Int rightCoord = thisCoord + Vector2Int.right;

                if (_grid.IsInsideGrid(topCoord))
                {
                    Tile toTile = _grid.Get(topCoord);
                    _grid.Swap(fromTile, toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile));

                    _grid.Swap(toTile, fromTile);

                    if (matches.Count > 0)
                    {
                        hintDir = GridDir.Up;
                        return false;
                    }
                }

                if (_grid.IsInsideGrid(botCoord))
                {
                    Tile toTile = _grid.Get(botCoord);
                    _grid.Swap(fromTile, toTile);

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
                    Tile toTile = _grid.Get(leftCoord);
                    _grid.Swap(fromTile, toTile);

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
                    Tile toTile = _grid.Get(rightCoord);
                    _grid.Swap(fromTile, toTile);

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

        private void SpawnAndAllocateTiles()
        {
            bool didDestroy = true;

            _tilesToMove = new Tile[_gridSizeX, _gridSizeY];


            for (int y = 0; y < _gridSizeY; y++)
            {
                int spawnStartY = 0;

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

                            MonoPool randomPool = _tilePoolsByPrefabID.Random();
                            Tile newTile = SpawnTile
                            (
                                randomPool,
                                _grid.CoordsToWorld(_transform, new Vector2Int(x, spawnPoint)),
                                thisCoord
                            );


                            _tilesToMove[thisCoord.x, thisCoord.y] = newTile;
                            break;
                        }

                        Vector2Int emptyCoords = new(x, y1);
                        Tile mostTopTile = _grid.Get(emptyCoords);

                        if (mostTopTile)
                        {
                            _grid.Set(null, mostTopTile.Coords);
                            _grid.Set(mostTopTile, thisCoord);

                            _tilesToMove[thisCoord.x, thisCoord.y] = mostTopTile;
                            break;
                        }
                    }
                }
            }


            StartCoroutine(RainDownRoutine());
        }

        private Tile SpawnTile(MonoPool randomPool, Vector3 spawnWorldPos, Vector2Int spawnCoord)
        {
            Tile newTile = randomPool.Request<Tile>();

            newTile.Teleport(spawnWorldPos);
            _grid.Set(newTile, spawnCoord);


            return newTile;
        }

        private Tile SpawnTile(int id, Vector3 worldPos, Vector2Int coords) => SpawnTile
            (_tilePoolsByPrefabID[id], worldPos, coords);

        private IEnumerator RainDownRoutine()
        {
            int longestDistY = 0;
            Tween longestTween = null;

            bool shouldWait = false;

            for (int y = 0; y < _gridSizeY; y++)
            {
                for (int x = 0; x < _gridSizeX; x++)
                {
                    Tile thisTile = _tilesToMove[x, y];

                    if (thisTile == false) continue;

                    Tween thisTween = thisTile.DOMove(_grid.CoordsToWorld(_transform, thisTile.Coords));

                    shouldWait = true;

                    if (longestDistY < y)
                    {
                        longestDistY = y;
                        longestTween = thisTween;
                    }
                }

                if (shouldWait)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }

            if (longestTween != null)
            {
                longestTween.onComplete += delegate
                {
                    List<Tile> newMatches = new();

                    if (HasAnyMatches(out _lastMatches))
                    {
                        StartDestroyRoutine();
                    }
                    else
                    {
                        IsGameOver(out _hintTile, out _hintDir);
                        GridEvents.InputStart?.Invoke();
                    }
                };
            }
            else
            {
                GridEvents.InputStart?.Invoke();
            }
        }

        private void StartDestroyRoutine()
        {
            if (_destroyRoutine  != null )
            {
                StopCoroutine(_destroyRoutine);
            }
            
            _destroyRoutine = StartCoroutine(DestroyRoutine());
        
        }

        private IEnumerator DestroyRoutine()
        {
            int groupCount = _lastMatches.Count;
            foreach (List<Tile> matches in _lastMatches)
            {
                IncScoreMulti();
                matches.DoToAll(DespawnTile);
                
                //TODO: Show Score Multi text in url as PunchScale
                
            GridEvents.MatchGroupDespawn?.Invoke(groupCount);
            
                yield return new WaitForSeconds(0.1f);
            } 
            
            SpawnAndAllocateTiles();
        }
        private void DespawnTile(Tile e)
        {
            _grid.Set(null, e.Coords);
            _tilePoolsByPrefabID[e.ID].DeSpawn(e);
        }


        private void DotileMoveAnim(Tile fromTile, Tile toTile, TweenCallback onComplete = null)
        {
            Vector3 fromTileWorldPos = _grid.CoordsToWorld(_transform, fromTile.Coords);
            fromTile.DOMove(fromTileWorldPos);
            Vector3 toTileWorldPos = _grid.CoordsToWorld(_transform, toTile.Coords);
            toTile.DOMove(toTileWorldPos, onComplete);
        }

        private void StartHintRoutine()
        {
            if (_hintRoutine != null)
            {
                StopCoroutine(_hintRoutine);
            }

            _hintRoutine = StartCoroutine(HintRoutineUpdate()
            );
        }

        private void StopHintRoutine()
        {
            if (_hintTile)
            {
                _hintTile.Teleport(_grid.CoordsToWorld(_transform, _hintTile.Coords));
            }
            if (_hintRoutine != null)
            {
                StopCoroutine(_hintRoutine);
                _hintRoutine = null; 
            }
        }
        
        private IEnumerator HintRoutineUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);
                TryShowHint(); 
            }
        }
        private void TryShowHint()
        {
            if (_hintTile)
            {
                Vector2Int gridMoveDir = _hintDir.ToVector();

                Vector3 moveCoords = _grid.CoordsToWorld(_transform, _hintTile.Coords + gridMoveDir);

               _hintTween = _hintTile.DOHint(moveCoords);
            }
        }

        private void RegisterEvents()
        {
            InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid;
            GridEvents.InputStart += OnInputStart;
            GridEvents.InputStop += OnInputStop;
        }

        private void OnInputStop()
        {
           StopHintRoutine(); 
        }

        private void OnInputStart()
        {
          StartHintRoutine();
          ResetScoreMulti();
        }

        private void ResetScoreMulti() {_scoreMulti = 0;}

        private void IncScoreMulti()
        {
            _scoreMulti++;
            
        }

        private void OnMouseDownGrid(Tile clickedTile, Vector3 dirVector)
        {
            _selectedTile = clickedTile;
            _mouseDownPos = dirVector;
            if (_hintTween.IsActive())
            {
                _hintTween.Complete();
            }
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

                if (!HasAnyMatches(out _lastMatches))
                {
                    GridEvents.InputStop?.Invoke();

                    DotileMoveAnim(_selectedTile, toTile, delegate
                    {
                        _grid.Swap(toTile, _selectedTile);

                        DotileMoveAnim(_selectedTile, toTile,
                            delegate { GridEvents.InputStart?.Invoke(); });
                    });
                }
                else
                {
                    GridEvents.InputStop?.Invoke();

                    DotileMoveAnim(_selectedTile, toTile,
                        delegate
                        {
                           StartDestroyRoutine();
                        });
                }
            }
        }

      

        private void UnRegisterEvents()
        {
            InputEvents.MouseDownGrid -= OnMouseDownGrid;
            InputEvents.MouseUpGrid -= OnMouseUpGrid;
            GridEvents.InputStart -= OnInputStart;
            GridEvents.InputStop -= OnInputStop;
        }
    }
}