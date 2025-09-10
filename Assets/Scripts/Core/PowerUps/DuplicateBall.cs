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

                Ball[] balls = FindObjectsByType<Ball>(FindObjectsSortMode.InstanceID);
                if (balls.Length != 0)
                {
                    foreach (Ball ball in balls)
                    {
                        if (GameSessionController.Instance.CurrentNumberOfBalls >= GameSessionController.Instance.MaxNumberOfBalls) break;

                        Ball newBall = Instantiate(ball, ball.transform.position, Quaternion.identity) as Ball;
                        newBall.Velocity = (ball.Velocity.normalized * -1 * Time.fixedDeltaTime * ball.MoveSpeed);
                        newBall.MoveSpeed = ball.MoveSpeed;
                        if (ball.IsBallOnFire)
                        {
                            newBall.IsBallOnFire = true;
                            newBall.ChangeBallSprite(newBall.IsBallOnFire);
                        }

                        GameSessionController.Instance.CurrentNumberOfBalls++;
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