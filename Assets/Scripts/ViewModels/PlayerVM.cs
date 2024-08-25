using System;
using Components;
using Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using IInitializable = Zenject.IInitializable;

namespace ViewModels
{
    [Serializable]
    public class PlayerVM: IInitializable, IDisposable
    {
        [Inject] private GridEvents GridEvents { get; set; }
        [Inject] private ProjectEvents ProjectEvents { get; set; }
        [ShowInInspector]public int MoveCount{ get; set; }
        [SerializeField] private int _level;
        
        public int Level
        {
            get => _level;
            set => _level = value;
        }
        
        public void Dispose()
        {
            GridEvents.PlayerMoved -= OnPlayerMoved;
        }
        
        public void Initialize()
        {
            GridEvents.PlayerMoved += OnPlayerMoved;
        }

        private void OnPlayerMoved()
        {
            MoveCount--;

            if (MoveCount < 0)
            {
                _level++;
                ProjectEvents.LevelComplete?.Invoke();
            }
        }

        
    }
}