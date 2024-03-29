using Controllers.Core;
using MVC.Enums;
using System;
using UnityEngine;

namespace Core.PowerUps
{
    public class ResetBall : PowerUp
    {
        /// <summary>
        /// Applies power up effect
        /// </summary>
        protected override void Apply()
        {
            try
            {
                Ball[] balls = FindObjectsOfType<Ball>();
                if (balls.Length != 0)
                {
                    foreach (Ball ball in balls)
                    {
                        ball.transform.localScale = Vector3.one;
                        ball.MoveSpeed = ball.DefaultSpeed;
                        ball.Velocity = (ball.Velocity.normalized * Time.fixedDeltaTime * ball.MoveSpeed);
                    }

                    GameSessionController.Instance.AddToScore(UnityEngine.Random.Range(100, 1000));
                    GameSessionController.Instance.ShowPowerUpName(LocalizationController.Instance.GetWord(LocalizationFields.powerups_resetballs));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}