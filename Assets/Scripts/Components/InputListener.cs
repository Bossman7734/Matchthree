using System;
using System.Linq;
using Components.UI.GameOver;
using Events;
using Extensions.System;
using Extensions.Unity;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Components
{
    public class InputListener : EventListenerMono
    {
        private const float ZoomDeltaThreshold = 0.01f;
        [Inject] private InputEvents InputEvents{get;set;}
        [Inject] private GridEvents GridEvents { get;set; }
        [Inject] private Camera Camera {get;set;}
        private RoutineHelper _InputRoutine;
        private float _lastDist;
        private int _lastTouchCount;


        private void Awake()
        {
            _InputRoutine = new RoutineHelper(this, null, InputUpdate);
        }

        
        private void InputUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray InputRay = Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(InputRay, 100f);

                Tile firstHitTile = null;

                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.TryGetComponent(out firstHitTile))break;
                }

                if (firstHitTile)
                {
                    InputEvents.MouseDownGrid?.Invoke(firstHitTile, InputRay.origin + InputRay.direction);
                    Debug.LogWarning(firstHitTile.transform.name);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Ray InputRay = Camera.ScreenPointToRay(Input.mousePosition);
                InputEvents.MouseUpGrid?.Invoke(InputRay.origin + InputRay.direction);
                
            }

            int touchCount = Input.touchCount;

            if (touchCount > 1)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                float currDist = (touch1.position - touch2.position).magnitude;

                if (_lastTouchCount < 2)
                {
                    _lastTouchCount = touchCount;

                    _lastDist = currDist;
                    return;
                }

                float distDelta = _lastDist - currDist;

                if (distDelta.Abs() >= ZoomDeltaThreshold)
                {
                    InputEvents.ZoomDelta?.Invoke(distDelta);
                }

                _lastTouchCount = touchCount;
                _lastDist = currDist;
            }
            else
            {
                _lastTouchCount = 0;
                _lastDist = 0;
            }
        }

        protected override void RegisterEvents()
        {
            GridEvents.InputStart += OnInputStart;
            GridEvents.InputStop += OnInputStop;
            GameOverEvents.MainMenüBTN += OnMainMenüBTN;
            GameOverEvents.TryAgainBTN += OnTryAgainBTN;
        }

        private void OnTryAgainBTN()
        {
            SceneManager.LoadScene("Main");
        }

        private void OnMainMenüBTN()
        {
            SceneManager.LoadScene("MainMenü");
        }
        
        private void OnInputStart()
        {
            _InputRoutine.StartCoroutine();
        }
        private void OnInputStop()
        {
            _InputRoutine.StopCoroutine();
        }

        protected override void UnRegisterEvents()
        {
            GridEvents.InputStart -= OnInputStart;
            GridEvents.InputStop -= OnInputStop;
        }
    }
}