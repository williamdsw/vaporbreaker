using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class ScoreboardRow : MonoBehaviour
    {
        // || Inspector References

        [Header("Required Columns")]
        [SerializeField] private TextMeshProUGUI orderColumnValue;
        [SerializeField] private TextMeshProUGUI scoreColumnValue;
        [SerializeField] private TextMeshProUGUI timeScoreColumnValue;
        [SerializeField] private TextMeshProUGUI bestComboColumnValue;
        [SerializeField] private TextMeshProUGUI momentColumnValue;
        [SerializeField] private Color32 oddColor;
        [SerializeField] private Color32 evenColor;

        // || Cached

        private Image image;

        private void Awake() => image = GetComponent<Image>();

        public void Set(int order, long score, long timeScore, long bestCombo, long moment, bool isOdd)
        {
            try
            {
                orderColumnValue.text = order.ToString();
                scoreColumnValue.text = Formatter.FormatToCurrency(score);
                timeScoreColumnValue.text = Formatter.GetEllapsedTimeInHours((int)timeScore);
                bestComboColumnValue.text = bestCombo.ToString();
                momentColumnValue.text = Formatter.FormatDateTimer(moment, "yyyy-MM-dd HH:mm:ss");
                image.color = (isOdd ? oddColor : evenColor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}