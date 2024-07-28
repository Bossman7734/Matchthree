using System;
using System.Linq;
using Components.UI.GameOver;
using Events;
using Extensions.Unity;
using Extensions.Unity.MonoHelper;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Components
{
    public class InputListener : EventListenerMono
    {
        [Inject] private InputEvents InputEvents{get;set;}
        [Inject] private GridEvents GridEvents { get;set; }
        [Inject] private Camera Camera {get;set;}
        private RoutineHelper _InputRoutine;


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