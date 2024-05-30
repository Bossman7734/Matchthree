using Events;
using UnityEngine;
using Zenject;

namespace Components
{
   public class TestCube : MonoBehaviour // Containerdan Class İsteme Çağırma...
   {
      [Inject] private ProjectEvents ProjectEvents { get; set; }

      private void OnEnable()
      {
         RegisterEvents();
      }
      private void OnDisable()
      {
         UnRegisterEvents();
      }
   
      private void RegisterEvents()
      {
         ProjectEvents.ProjectStarted += ProjectStarted;
      }

      private void ProjectStarted()
      {
         Debug.LogWarning("VAR");
      }
   
      private void UnRegisterEvents()
      {
         ProjectEvents.ProjectStarted -= ProjectStarted;
      }
   }
}
