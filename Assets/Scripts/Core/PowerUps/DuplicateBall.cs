using Controllers.Core;
using MVC.Enums;
using System;
using UnityEngine;

namespace Core.PowerUps
{
    public class DuplicateBall : PowerUp
    {
        /// <summary>
        /// Applies power up effect
        /// </summary>
        protected override void Apply()
        {
            try
            {
                if (GameSessionController.Instance.CurrentNumberOfBalls >= GameSessionController.Instance.MaxNumberOfBalls) return;

                Ball[] balls = FindObjectsOfType<Ball>();
                if (balls.Length != 0)
                {
                    foreach (Ball ball in balls)
                    {
                        Ball newBall = Instantiate(ball, ball.transform.position, Quaternion.identity) as Ball;
                        newBall.Velocity = (ball.Velocity * -1);
                        newBall.MoveSpeed = ball.MoveSpeed;
                        newBall.IsBallOnFire = ball.IsBallOnFire;
                        newBall.ChangeBallSprite(newBall.IsBallOnFire);
                    }

                    GameSessionController.Instance.AddToScore(UnityEngine.Random.Range(500, 2500));
                    GameSessionController.Instance.ShowPowerUpName(LocalizationController.Instance.GetWord(LocalizationFields.powerups_duplicateball));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}