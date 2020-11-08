using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    private Animator animator;
    private CursorController cursorController;
    private FullScreenBackground fullScreenBackground;
    private GameSession gameSession;
    private GameState newGameState;

    //--------------------------------------------------------------------------------//

    private void Start () 
    {
        // Find Component
        animator = this.GetComponent<Animator>();

        // Find Other Objects
        gameSession = FindObjectOfType<GameSession>();
        cursorController = FindObjectOfType<CursorController>();
        fullScreenBackground = FindObjectOfType<FullScreenBackground>();

        if (SceneManagerController.GetActiveSceneIndex () > 2)
        {
            CreateFadeInEvents ();
            CreateFadeOutEvents ();
        }
    }

    //--------------------------------------------------------------------------------//

    private void CreateFadeInEvents ()
    {
        AnimationClip fadeInClip = animator.runtimeAnimatorController.animationClips[0];
        fadeInClip.events = null;

        // First frame event
        AnimationEvent firstFrameEvent = new AnimationEvent ();
        firstFrameEvent.intParameter = 2;
        firstFrameEvent.time = 0f;
        firstFrameEvent.functionName = "DefineGameState";
        fadeInClip.AddEvent (firstFrameEvent);

        fadeInClip = animator.runtimeAnimatorController.animationClips[0];

        // Last frame event
        AnimationEvent lastFrameEvent = new AnimationEvent ();
        lastFrameEvent.intParameter = 0;
        lastFrameEvent.time = 1f;
        lastFrameEvent.functionName = "DefineGameState";
        fadeInClip.AddEvent (lastFrameEvent);
    }

    private void CreateFadeOutEvents ()
    {
        AnimationClip fadeOutClip = animator.runtimeAnimatorController.animationClips[1];
        fadeOutClip.events = null;

        // First frame event
        AnimationEvent firstFrameEvent = new AnimationEvent ();
        firstFrameEvent.intParameter = 2;
        firstFrameEvent.time = 0f;
        firstFrameEvent.functionName = "DefineGameState";
        fadeOutClip.AddEvent (firstFrameEvent);

        // Last frame event
        AnimationEvent lastFrameEvent = new AnimationEvent ();
        lastFrameEvent.time = 1f;
        lastFrameEvent.functionName = "CallResetLevel";
        fadeOutClip.AddEvent (lastFrameEvent);
    }

    public void ResetFadeOutEventsToLevelMenu ()
    {
        AnimationClip fadeOutClip = animator.runtimeAnimatorController.animationClips[1];
        fadeOutClip.events = null;

        // Last frame event
        AnimationEvent lastFrameEvent = new AnimationEvent ();
        lastFrameEvent.time = 1f;
        lastFrameEvent.functionName = "CallLevelMenu";
        fadeOutClip.AddEvent (lastFrameEvent);
    }

    public void ResetAnimationFunctions ()
    {
        if (!animator) { animator = this.GetComponent<Animator>(); }
        
        AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in animationClips) { clip.events = null; }
    }

    public float GetFadeOutLength ()
    {
        return animator.runtimeAnimatorController.animationClips[1].length;
    }

    public void FadeToLevel ()
    {
        animator.SetTrigger ("FadeOut");
    }

    public void CallResetLevel ()
    {
        // Cancels
        if (!gameSession) { return; }

        animator.Rebind ();
        gameSession.ResetLevel ();
    }

    public void CallLevelMenu ()
    {
        // Cancels
        if (!gameSession || !cursorController || !fullScreenBackground) { return; }

        animator.Rebind ();
        gameSession.ResetGame (SceneManagerController.GetSelectLevelsSceneName ());
    }

    public void DefineGameState (int gameStateInt)
    {
        // Cancels
        if (!gameSession) { return; }

        if (gameStateInt == 0)
        {
            newGameState = GameState.GAMEPLAY;
        }
        else if (gameStateInt == 1)
        {
            newGameState = GameState.PAUSE;
        }
        else if (gameStateInt == 2)
        {
            newGameState = GameState.TRANSITION;
        }

        gameSession.SetActualGameState (newGameState);
    }
}