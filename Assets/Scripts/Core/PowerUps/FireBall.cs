using Controllers.Core;
using MVC.Enums;
using System;

namespace Core.PowerUps
{
    public class FireBall : PowerUp
    {
        /// <summary>
        /// Applies power up effect
        /// </summary>
        protected override void Apply()
        {
            try
            {
                GameSessionController.Instance.MakeFireBalls();
                GameSessionController.Instance.AddToScore(UnityEngine.Random.Range(-10000, 10000));
                GameSessionController.Instance.ShowPowerUpName(LocalizationController.Instance.GetWord(LocalizationFields.powerups_fireball));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}