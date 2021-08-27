using Luminosity.IO;
using System.Collections;
using TMPro;
using UnityEngine;
using Utilities;

public class Credits : MonoBehaviour
{
    // Config
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private GameObject jumpText;
    private float movementY = 30f;

    // State
    private bool canMoveText = false;
    private bool canCalculateTime = true;
    private bool canPressButton = false;
    private float ellapsedTime = 0f;
    private float maxEllapsedTime = 144f;
    private float timeToWait = 5f;

    // Cached
    private RectTransform creditsRectTransform;
    private AudioController audioController;
    private GameStatusController gameStatusController;
    private FadeEffect fadeEffect;

    private void Start()
    {
        if (!creditsText) return;

        // Components
        creditsRectTransform = creditsText.GetComponent<RectTransform>();

        // Other
        audioController = FindObjectOfType<AudioController>();
        gameStatusController = FindObjectOfType<GameStatusController>();
        fadeEffect = FindObjectOfType<FadeEffect>();

        audioController.ChangeMusic(audioController.AllLoopedSongs[3], false, "", true, false);
        creditsText.text = FileManager.LoadAsset(FileManager.OtherFolderPath, FileManager.CreditsPath);

        StartCoroutine(WaitToMoveText());
    }

    private void Update()
    {
        if (canMoveText)
        {
            MoveText();
            CaptureStartInput();
        }

        if (canCalculateTime)
        {
            ellapsedTime += Time.deltaTime;

            if (ellapsedTime >= maxEllapsedTime)
            {
                canCalculateTime = false;
                canMoveText = false;
                StartCoroutine(CallNextScene(SceneManagerController.GetMainMenuSceneName(), true));
            }
        }
    }

    // Moves the text up
    private void MoveText()
    {
        Vector3 newPosition = new Vector3(0, movementY * Time.deltaTime, 0);
        creditsRectTransform.transform.position += newPosition;
    }

    // Captures Pause Button
    private void CaptureStartInput()
    {
        if (canPressButton)
        {
            if (InputManager.anyKeyDown)
            {
                canPressButton = false;
                StartCoroutine(CallNextScene(SceneManagerController.GetMainMenuSceneName(), false));
            }
        }
    }

    private IEnumerator WaitToMoveText()
    {
        yield return new WaitForSecondsRealtime(timeToWait);
        canMoveText = true;
        canPressButton = true;
        jumpText.SetActive(true);
    }

    // Wait fade out length to fade out to next scene
    private IEnumerator CallNextScene(string nextSceneName, bool isTheEnd)
    {
        // Stop 
        jumpText.SetActive(false);
        audioController.StopMusic();

        // Case end of text
        if (isTheEnd)
        {
            yield return new WaitForSecondsRealtime(timeToWait * 2f);
        }

        // Fade Out
        float fadeOutLength = fadeEffect.GetFadeOutLength();
        fadeEffect.FadeToLevel();
        yield return new WaitForSecondsRealtime(fadeOutLength);
        gameStatusController.SetNextSceneName(nextSceneName);
        gameStatusController.SetCameFromLevel(false);
        SceneManagerController.CallScene(SceneManagerController.GetLoadingSceneName());
    }
}