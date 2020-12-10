using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject[] allMenus;

    [Header("Buttons")]
    [SerializeField] private Button[] allButtons;

    [Header("Default Selected Buttons")]
    [SerializeField] private Button[] allDefaultButtons;

    [Header("Other UI")]
    [SerializeField] private GameObject questionText;
    [SerializeField] private Button[] questionAnswerButtons;

    [Header("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> uiLabels = new List<TextMeshProUGUI>();

    // State 
    private bool hasSavedGame = false;

    // Cached
    private AudioController audioController;
    private CanvasGroup canvasGroup;
    private FadeEffect fadeEffect;
    private GameStatusController gameStatusController;
    private LocalizationController localizationController;

    private void Start()
    {
        // Find Objects
        audioController = FindObjectOfType<AudioController>();
        canvasGroup = FindObjectOfType<CanvasGroup>();
        fadeEffect = FindObjectOfType<FadeEffect>();
        gameStatusController = FindObjectOfType<GameStatusController>();
        localizationController = FindObjectOfType<LocalizationController>();

        // Play music
        audioController.ChangeMusic(audioController.AllLoopedSongs[0], false, "", true, false);

        // Resets for animation works
        Time.timeScale = 1f;
        hasSavedGame = ProgressManager.HasProgress();
        Cursor.visible = true;

        TranslateLabels();
        BindingClickEvents();
        InputManager.Load();
    }

    private void Update()
    {
        CaptureCancelButton();
    }

    private void BindingClickEvents()
    {
        if (allMenus.Length == 0 || allButtons.Length == 0 || allDefaultButtons.Length == 0) return;

        // GOTO LEVELS SCENE OR NEXT PANEL
        allButtons[0].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != allButtons[0].gameObject) return;

            if (hasSavedGame)
            {
                allMenus[0].SetActive(false);
                allMenus[1].SetActive(true);
                allDefaultButtons[1].Select();
            }
            else
            {
                audioController.StopMusic();
                StartCoroutine(CallNextScene(SceneManagerController.GetSelectLevelsSceneName()));
            }
        });

        // CONTINUE BUTTON
        allButtons[1].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != allButtons[1].gameObject) return;

            if (hasSavedGame && allMenus[1].activeSelf)
            {
                audioController.StopMusic();
                StartCoroutine(CallNextScene(SceneManagerController.GetSelectLevelsSceneName()));
            }
        });

        // DELETE PROGRESS
        allButtons[2].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != allButtons[2].gameObject) return;

            if (hasSavedGame && allMenus[1].activeSelf)
            {
                allButtons[1].interactable = false;
                allButtons[2].interactable = false;
                questionText.SetActive(true);
                questionAnswerButtons[0].Select();
            }
        });

        // NO BUTTON
        questionAnswerButtons[0].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != questionAnswerButtons[0].gameObject) return;

            if (questionText.activeSelf)
            {
                allButtons[1].interactable = true;
                allButtons[2].interactable = true;
                questionText.SetActive(false);
                allDefaultButtons[1].Select();
            }
        });

        // YES BUTTON
        questionAnswerButtons[1].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != questionAnswerButtons[1].gameObject) return;

            if (questionText.activeSelf)
            {
                allButtons[1].interactable = true;
                allButtons[2].interactable = true;
                ProgressManager.DeleteProgress();
                hasSavedGame = false;
                questionText.SetActive(false);
                allMenus[1].SetActive(false);
                allMenus[0].SetActive(true);
                allDefaultButtons[0].Select();
            }
        });

        // OPTIONS MENU
        allButtons[3].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != allButtons[3].gameObject) return;

            allMenus[0].SetActive(false);
            allMenus[2].SetActive(true);
            allDefaultButtons[2].Select();
        });

        // LANGUAGES MENU
        allButtons[4].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != allButtons[4].gameObject) return;

            allMenus[0].SetActive(false);
            allMenus[3].SetActive(true);
            allDefaultButtons[3].Select();
        });

        // POWER UPS SCENE
        allButtons[5].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != allButtons[5].gameObject) return;

            audioController.StopMusic();
            StartCoroutine(CallNextScene(SceneManagerController.GetPowerUpsSceneName()));
        });

        // SOUNDTRACK SCENE
        allButtons[6].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != allButtons[6].gameObject) return;

            audioController.StopMusic();
            StartCoroutine(CallNextScene(SceneManagerController.GetSoundtrackSceneName()));
        });

        // CREDITS SCENE
        allButtons[7].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != allButtons[7].gameObject) return;

            audioController.StopMusic();
            StartCoroutine(CallNextScene(SceneManagerController.GetCreditsSceneName()));
        });

        // QUIT GAME
        allButtons[8].onClick.AddListener(() =>
        {
            if (EventSystem.current.currentSelectedGameObject != allButtons[8].gameObject) return;

            SceneManagerController.QuitGame();
        });

        // UI Sounds
        foreach (Button button in allButtons)
        {
            button.onClick.AddListener(() =>
            {
                audioController.PlaySFX(audioController.UiSubmit, audioController.GetMaxSFXVolume());
            });
        }

        // UI Sounds
        foreach (Button button in questionAnswerButtons)
        {
            button.onClick.AddListener(() =>
            {
                audioController.PlaySFX(audioController.UiSubmit, audioController.GetMaxSFXVolume());
            });
        }
    }

    // Translate labels based on choosed language
    public void TranslateLabels()
    {
        // CANCELS
        if (!localizationController) return;

        List<string> labels = new List<string>();
        foreach (string label in localizationController.GetMainMenuLabels())
        {
            labels.Add(label);
        }

        foreach (string label in localizationController.GetProgressLabels())
        {
            labels.Add(label);
        }

        // Checks
        if (labels.Count == 0 || uiLabels.Count == 0) return;

        // Set text and size
        for (int index = 0; index < labels.Count; index++)
        {
            uiLabels[index].SetText(labels[index]);

            if (index > 1 && index < 10)
            {
                Transform parent = uiLabels[index].transform.parent;
                RectTransform rectTransform = parent.GetComponent<RectTransform>();
                int numberOfLetters = labels[index].Length;
                float width = (index == 9 ? 50f : 80f);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, numberOfLetters * width);
            }
        }
    }

    public void MakeSelectOnPointerEnter(Button button)
    {
        if (!button || !button.interactable) return;
        button.Select();
    }

    // Capture Cancel Button on situations
    private void CaptureCancelButton()
    {
        // QUESTION TEXT
        if (questionText.activeSelf)
        {
            if (InputManager.GetButtonDown("UI_Cancel"))
            {
                allButtons[1].interactable = true;
                allButtons[2].interactable = true;
                audioController.PlaySFX(audioController.UiCancel, audioController.GetMaxSFXVolume());
                questionText.SetActive(false);
                allDefaultButtons[1].Select();
            }

            return;
        }

        // CONTINUE MENU
        if (allMenus[1].activeSelf)
        {
            if (InputManager.GetButtonDown("UI_Cancel"))
            {
                audioController.PlaySFX(audioController.UiCancel, audioController.GetMaxSFXVolume());
                allMenus[1].SetActive(false);
                allMenus[0].SetActive(true);
                allDefaultButtons[0].Select();
            }

            return;
        }
    }

    // Wait fade out length to fade out to next scene
    private IEnumerator CallNextScene(string nextSceneName)
    {
        canvasGroup.interactable = false;

        // Fade Out effect
        float fadeOutLength = fadeEffect.GetFadeOutLength();
        fadeEffect.FadeToLevel();
        yield return new WaitForSecondsRealtime(fadeOutLength);
        gameStatusController.SetNextSceneName(nextSceneName);
        gameStatusController.SetCameFromLevel(false);
        SceneManagerController.CallScene(SceneManagerController.GetLoadingSceneName());
    }
}