using System;
using DG.Tweening;
using Extensions.DoTween;
using Extensions.Unity;
using UnityEngine;


namespace Components
{
    public class Tile : MonoBehaviour, ITileGrid,IPoolObj, ITweenContainerBind
    {
        
        
        public Vector2Int Coords => _coords;
        public int ID => _id;

        [SerializeField] private Vector2Int _coords;
        [SerializeField] private int _id;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _transform;


        public MonoPool MyPool { get; set; }
        public ITweenContainer TweenContainer { get; set; }

        private void Awake()
        {
            TweenContainer = TweenContain.Install(this);
        }

        private void OnDisable()
        {
            TweenContainer.Clear();
        }

        private void OnMouseDown() {}


        void ITileGrid.SetCoord(Vector2Int coord)
        {
            _coords = coord;

        }

        public void Teleport(Vector3 worldPos)
        {
            _transform.position = worldPos;
        }

        void ITileGrid.SetCoord(int x, int y)
        {
            _coords = new Vector2Int(x, y);
        }

        public void AfterCreate()
        {
            
        }

        public void BeforeDeSpawn()
        {
             
        }

        public void TweenDelayedDeSpawn(Func<bool> onComplete)
        {
            
        }

        public void AfterSpawn()
        {
           //RESET METHOD (Ressurrect)
        }

        public void Construct(Vector2Int coords)
        {
            _coords = coords;
        }

        public Tween DOMove(Vector3 worldPos, TweenCallback onComplete = null)
        { 
           TweenContainer.AddedTween = _transform.DOMove(worldPos, 1f);
           TweenContainer.AddedTween.onComplete += onComplete;

           return TweenContainer.AddedTween;

        }

        public void DOHint(GridDir gridDir)
        {
            //TODO: Later...
        }
    }

    public interface ITileGrid
    {
        void SetCoord(Vector2Int coord);
        void SetCoord(int x, int y);
        Vector2Int Coords { get; }
    }
}