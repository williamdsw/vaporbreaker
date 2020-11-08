using System.Collections;
using UnityEngine;

public class Loading : MonoBehaviour
{
    // Config
    private float timeToWait = 1f;

    // State
    private AsyncOperation operation;

    // Cached
    private GameStatusController gameStatusController;
    private AudioController audioController;
    private FadeEffect fadeEffect;

    // Destructables singletons
    private CursorController cursorController;
    private FullScreenBackground fullScreenBackground;

    private void Start () 
    {
        // Find Objects
        gameStatusController = FindObjectOfType<GameStatusController>();
        audioController = FindObjectOfType<AudioController>();
        cursorController = FindObjectOfType<CursorController>();
        fullScreenBackground = FindObjectOfType<FullScreenBackground>();
        fadeEffect = FindObjectOfType<FadeEffect>();

        // Destroy some singletons
        if (cursorController) { cursorController.DestroyInstance (); }
        if (fullScreenBackground) { fullScreenBackground.DestroyInstance (); }

        StartCoroutine (CallNextScene ());
    }

    private IEnumerator CallNextScene ()
    {
        // Fade Out effect
        yield return new WaitForSecondsRealtime (timeToWait);
        float fadeOutLength = fadeEffect.GetFadeOutLength ();
        fadeEffect.FadeToLevel ();
        yield return new WaitForSecondsRealtime (fadeOutLength);

        // Calls next scene
        string nextSceneName = gameStatusController.GetNextSceneName ();
        operation = SceneManagerController.CallSceneAsync (nextSceneName);
    }
}