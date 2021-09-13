using System.Collections;
using System.Collections.Generic;
using Controllers.Core;
using Core;
using Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteController : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject levelCompletedPanel;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private List<TextMeshProUGUI> labelsText;
    [SerializeField] private List<TextMeshProUGUI> valuesText;
    [SerializeField] private TextMeshProUGUI newScoreText;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI>();
    private string returningToLevelsText;

    // Config
    [SerializeField] private float defaultSecondsValue = 1f;

    // Data / State
    private int currentScore = 0;
    private int timeScore = 0;
    private int blocksDestroyed = 0;
    private int totalOfBlocks = 0;
    private int bestCombo = 0;
    private int numberOfDeaths = 0;
    private int numberOfBalls = 0;
    private int totalScore = 0;
    private float countdownTimer = 3f;

    // Cached
    private FullScreenBackground fullScreenBackground;
    private Ball[] balls;

    public static LevelCompleteController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Find objects
        fullScreenBackground = FindObjectOfType<FullScreenBackground>();

        levelCompletedPanel.SetActive(false);
        //TranslateLabels();
        DefaultUIValues();
    }

   

    private void DefaultUIValues()
    {
        // Label text
        foreach (TextMeshProUGUI labelText in labelsText)
        {
            GameObject parent = labelText.gameObject.transform.parent.gameObject;
            parent.SetActive(false);
        }

        // Value text
        foreach (TextMeshProUGUI valueText in valuesText)
        {
            GameObject parent = valueText.gameObject.transform.parent.gameObject;
            parent.SetActive(false);
            valueText.text = string.Empty;
        }

        newScoreText.enabled = false;
        countdownText.text = string.Empty;
        countdownText.enabled = false;
    }

    public void CallLevelComplete(float timeScore, int bestCombo, int currentScore)
    {
        // Finds the balls
        balls = FindObjectsOfType<Ball>();

        // Passing values
        this.timeScore = Mathf.FloorToInt(timeScore);
        this.bestCombo = bestCombo;
        this.currentScore = currentScore;
        this.numberOfBalls = balls.Length;

        StartCoroutine(LevelComplete());
    }

    private void CalculateTotalScore()
    {
        if (valuesText.Count == 0) return;

        // Calculates total time
        int totalTimeScore = 0;

        if (totalScore > 60)
        {
            totalTimeScore = 0;
        }
        else if (timeScore > 50 && totalScore <= 60)
        {
            totalTimeScore = 20000;
        }
        else if (timeScore > 40 && totalScore <= 50)
        {
            totalTimeScore = 30000;
        }
        else if (timeScore > 40 && totalScore <= 50)
        {
            totalTimeScore = 40000;
        }
        else if (timeScore > 30 && totalScore <= 40)
        {
            totalTimeScore = 50000;
        }
        else if (timeScore > 20 && totalScore <= 30)
        {
            totalTimeScore = 60000;
        }
        else if (timeScore > 10 && totalScore <= 20)
        {
            totalTimeScore = 80000;
        }
        else if (timeScore > 0 && totalScore <= 10)
        {
            totalTimeScore = 100000;
        }

        totalScore = currentScore + totalTimeScore;

        // Calculate blocks
        totalScore += (blocksDestroyed == totalOfBlocks ? blocksDestroyed * 100 : 0);

        // Calculate deaths
        totalScore += (numberOfDeaths == 0 ? 100000 : numberOfDeaths * -20000);
        totalScore += (numberOfBalls > 0 ? numberOfBalls * 50000 : 0);

        // Calculate lives
        totalScore += (numberOfDeaths * 25000);
        totalScore += (numberOfBalls * 50000);

        string[] values =
        {
            currentScore.ToString(), totalTimeScore.ToString(), string.Concat(blocksDestroyed, " / ", totalOfBlocks),
            bestCombo.ToString(), numberOfDeaths.ToString(), numberOfBalls.ToString(), totalScore.ToString()
        };

        for (int index = 0; index < valuesText.Count; index++)
        {
            valuesText[index].text = values[index];
        }
    }

    private IEnumerator LevelComplete()
    {
        // Stop any ball
        foreach (Ball ball in balls)
        {
            ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        AudioController.Instance.StopMusic();
        CalculateTotalScore();

        // Pass values
        GameStatusController.Instance.NewScore = totalScore;
        GameStatusController.Instance.NewTimeScore = timeScore;
        GameStatusController.Instance.IsLevelCompleted = true;

        // Plays success sound
        yield return new WaitForSecondsRealtime(defaultSecondsValue);
        AudioController.Instance.PlaySFX(AudioController.Instance.SuccessEffect, AudioController.Instance.MaxSFXVolume);

        // Show panel
        yield return new WaitForSecondsRealtime(defaultSecondsValue);
        levelCompletedPanel.SetActive(true);

        // Shows each text
        yield return new WaitForSecondsRealtime(defaultSecondsValue);
        for (int index = 0; index < labelsText.Count; index++)
        {
            AudioController.Instance.PlaySFX(AudioController.Instance.HittingFaceSound, AudioController.Instance.MaxSFXVolume);
            GameObject labelParent = labelsText[index].gameObject.transform.parent.gameObject;
            GameObject valueParent = valuesText[index].gameObject.transform.parent.gameObject;
            labelParent.SetActive(true);
            valueParent.SetActive(true);
            yield return new WaitForSecondsRealtime(defaultSecondsValue / 2);
        }

        // Case have a new score
        if (totalScore > GameStatusController.Instance.OldScore)
        {
            AudioController.Instance.PlaySFX(AudioController.Instance.NewScoreEffect, AudioController.Instance.MaxSFXVolume);
            newScoreText.enabled = true;
            newScoreText.GetComponent<FlashTextEffect>().enabled = true;
        }

        // Countdown
        yield return new WaitForSecondsRealtime(defaultSecondsValue * 2);
        countdownText.enabled = true;
        for (int countdownSeconds = 3; countdownSeconds > 0; countdownSeconds--)
        {
            countdownText.text = string.Concat(returningToLevelsText, " ", countdownSeconds);
            yield return new WaitForSecondsRealtime(defaultSecondsValue);
        }

        // Calls fade
        FadeEffect.Instance.ResetAnimationFunctions();
        float fadeOutLength = FadeEffect.Instance.GetFadeOutLength();
        FadeEffect.Instance.FadeToLevel();
        yield return new WaitForSecondsRealtime(fadeOutLength);
        GameSessionController.Instance.GotoScene(SceneManagerController.SelectLevelsSceneName);
    }
}