using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefaultLayoutMenu : MonoBehaviour
{
    [Header("Labels to Translate")]
    [SerializeField] private List<TextMeshProUGUI> keyboardUILabels = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> gamepadUILabels = new List<TextMeshProUGUI>();

    // Cached
    private LocalizationController localizationController;

    private void Start()
    {
        localizationController = FindObjectOfType<LocalizationController>();
        TranslateLabels();
    }

    // Translate labels based on choosed language
    public void TranslateLabels()
    {
        if (!localizationController) return;

        List<string> keyboardLabels = localizationController.GetDefaultKeyboardLayoutLabels();
        if (keyboardLabels.Count == 0 || keyboardUILabels.Count == 0 || keyboardLabels.Count != keyboardUILabels.Count) return;
        for (int index = 0; index < keyboardLabels.Count; index++)
        {
            keyboardUILabels[index].SetText(keyboardLabels[index]);
        }

        List<string> gamepadLabels = localizationController.GetDefaultGamepadLayoutLabels();
        if (gamepadLabels.Count == 0 || gamepadUILabels.Count == 0 || gamepadLabels.Count != gamepadUILabels.Count) return;
        for (int index = 0; index < gamepadLabels.Count; index++)
        {
            gamepadUILabels[index].SetText(gamepadLabels[index]);
        }
    }
}