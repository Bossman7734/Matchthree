using System;
using System.Linq;
using Events;
using Extensions.Unity;
using UnityEngine;
using Zenject;

namespace Components
{
    public class InputListener : MonoBehaviour
    {
        [Inject] private InputEvents InputEvents{get;set;}
        [Inject] private Camera Camera {get;set;}
        private RoutineHelper _InputRoutine;

        private void Awake()
        {
            _InputRoutine = new RoutineHelper(this, null, InputUpdate);
        }

        private void Start()
        {
            _InputRoutine.StartCoroutine();
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
        
    }
}