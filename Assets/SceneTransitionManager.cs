using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitionManager : MonoBehaviour
{
    public void SwitchToMainMenu()
    {
        // Kill all active tweens to prevent missing reference errors
        DOTween.KillAll();

        // Optionally, you can fade out or perform some other transition effect here

        // Load the MainMenu scene
        SceneManager.LoadScene("MainMenü");
    }
}