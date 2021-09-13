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
                        Rigidbody2D ballRB = ball.GetComponent<Rigidbody2D>();
                        float defaultSpeed = ball.DefaultSpeed;
                        ball.transform.localScale = Vector3.one;
                        ball.MoveSpeed = defaultSpeed;
                        ballRB.velocity = (ballRB.velocity.normalized * Time.deltaTime * defaultSpeed);
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