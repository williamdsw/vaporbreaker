using Controllers.Core;
using MVC.Enums;
using System;
using UnityEngine;

namespace Core.PowerUps
{
    public class SizeBall : PowerUp
    {
        [Header("Additional Required Configuration")]
        [SerializeField] private bool makeBigger = false;

        /// <summary>
        /// Applies power up effect
        /// </summary>
        protected override void Apply() => Define(makeBigger);

        /// <summary>
        /// Define ball size
        /// </summary>
        /// <param name="makeBigger"> Is to make it bigger ? </param>
        private void Define(bool makeBigger)
        {
            try
            {
                Ball[] balls = FindObjectsOfType<Ball>();
                if (balls.Length != 0)
                {
                    foreach (Ball ball in balls)
                    {
                        Vector3 newLocalScale = ball.transform.localScale;
                        if (makeBigger)
                        {
                            if (newLocalScale.x < ball.MinMaxLocalScale.y)
                            {
                                newLocalScale *= 2f;
                            }
                        }
                        else
                        {
                            if (newLocalScale.x > ball.MinMaxLocalScale.x)
                            {
                                newLocalScale /= 2f;
                            }
                        }

                        Debug.Log(ball.transform.localScale);
                        ball.transform.localScale = newLocalScale;
                        Debug.Log(newLocalScale);
                        Debug.Log(ball.transform.localScale);
                    }

                    Vector2Int minMaxScore = new Vector2Int(makeBigger ? 0 : 1000, makeBigger ? 1000 : 5000);
                    LocalizationFields field = (makeBigger ? LocalizationFields.powerups_ballbigger : LocalizationFields.powerups_ballsmaller);
                    GameSessionController.Instance.AddToScore(UnityEngine.Random.Range(minMaxScore.x, minMaxScore.y));
                    GameSessionController.Instance.ShowPowerUpName(LocalizationController.Instance.GetWord(field));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}