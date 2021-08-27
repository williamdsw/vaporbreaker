using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    private class FunctionNames
    {
        public static string CallLevelMenu => "CallLevelMenu";
        public static string CallResetLevel => "CallResetLevel";
        public static string DefineGameState => "DefineGameState";
    }

    private Animator animator;
    private CursorController cursorController;
    private FullScreenBackground fullScreenBackground;
    private GameSession gameSession;
    private GameState newGameState;

    // || Properties

    public static FadeEffect Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        animator = this.GetComponent<Animator>();
    }

    private void Start()
    {
        // Find Other Objects
        gameSession = FindObjectOfType<GameSession>();
        cursorController = FindObjectOfType<CursorController>();
        fullScreenBackground = FindObjectOfType<FullScreenBackground>();

        if (SceneManagerController.GetActiveSceneIndex() > 2)
        {
            CreateFadeInEvents();
            CreateFadeOutEvents();
        }
    }

    private void CreateFadeInEvents()
    {
        AnimationClip fadeInClip = animator.runtimeAnimatorController.animationClips[0];
        fadeInClip.events = null;

        // First frame event
        AnimationEvent firstFrameEvent = new AnimationEvent();
        firstFrameEvent.intParameter = 2;
        firstFrameEvent.time = 0f;
        firstFrameEvent.functionName = "DefineGameState";
        fadeInClip.AddEvent(firstFrameEvent);

        fadeInClip = animator.runtimeAnimatorController.animationClips[0];

        // Last frame event
        AnimationEvent lastFrameEvent = new AnimationEvent();
        lastFrameEvent.intParameter = 0;
        lastFrameEvent.time = 1f;
        lastFrameEvent.functionName = "DefineGameState";
        fadeInClip.AddEvent(lastFrameEvent);
    }

    private void CreateFadeOutEvents()
    {
        AnimationClip fadeOutClip = animator.runtimeAnimatorController.animationClips[1];
        fadeOutClip.events = null;

        // First frame event
        AnimationEvent firstFrameEvent = new AnimationEvent();
        firstFrameEvent.intParameter = 2;
        firstFrameEvent.time = 0f;
        firstFrameEvent.functionName = "DefineGameState";
        fadeOutClip.AddEvent(firstFrameEvent);

        // Last frame event
        AnimationEvent lastFrameEvent = new AnimationEvent();
        lastFrameEvent.time = 1f;
        lastFrameEvent.functionName = "CallResetLevel";
        fadeOutClip.AddEvent(lastFrameEvent);
    }

    public void ResetFadeOutEventsToLevelMenu()
    {
        AnimationClip fadeOutClip = animator.runtimeAnimatorController.animationClips[1];
        fadeOutClip.events = null;

        // Last frame event
        AnimationEvent lastFrameEvent = new AnimationEvent();
        lastFrameEvent.time = 1f;
        lastFrameEvent.functionName = "CallLevelMenu";
        fadeOutClip.AddEvent(lastFrameEvent);
    }

    public void ResetAnimationFunctions()
    {
        if (!animator)
        {
            animator = this.GetComponent<Animator>();
        }

        AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in animationClips)
        {
            clip.events = null;
        }
    }

    public float GetFadeOutLength()
    {
        return (animator ? animator.runtimeAnimatorController.animationClips[1].length : 0f);
    }

    public void FadeToLevel()
    {
        if (!animator) return;
        animator.SetTrigger("FadeOut");
    }

    public void CallResetLevel()
    {
        if (!gameSession) return;
        animator.Rebind();
        gameSession.ResetLevel();
    }

    public void CallLevelMenu()
    {
        if (!gameSession || !cursorController || !fullScreenBackground) return;
        animator.Rebind();
        gameSession.ResetGame(SceneManagerController.GetSelectLevelsSceneName());
    }

    public void DefineGameState(int gameStateInt)
    {
        if (!gameSession) return;

        switch (gameStateInt)
        {
            case 0: newGameState = GameState.GAMEPLAY; break;
            case 1: newGameState = GameState.PAUSE; break;
            case 2: newGameState = GameState.TRANSITION; break;
            default: break;
        }

        gameSession.SetActualGameState(newGameState);
    }
}