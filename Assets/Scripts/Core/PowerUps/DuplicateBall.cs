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
                        Rigidbody2D ballRB = ball.GetComponent<Rigidbody2D>();
                        Ball newBall = Instantiate(ball, ball.transform.position, Quaternion.identity) as Ball;
                        Rigidbody2D newBallRB = newBall.GetComponent<Rigidbody2D>();
                        newBallRB.velocity = (ballRB.velocity.normalized * -1 * Time.deltaTime * ball.MoveSpeed);
                        newBall.MoveSpeed = ball.MoveSpeed;
                        if (ball.IsBallOnFire)
                        {
                            newBall.IsBallOnFire = true;
                            newBall.ChangeBallSprite(newBall.IsBallOnFire);
                        }
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