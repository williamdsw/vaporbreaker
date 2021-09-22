using Controllers.Core;
using MVC.Enums;
using System;

namespace Core.PowerUps
{
    public class ResetPaddle : PowerUp
    {
        /// <summary>
        /// Applies power up effect
        /// </summary>
        protected override void Apply()
        {
            try
            {
                if (paddle != null)
                {
                    paddle.ResetPaddle();
                    GameSessionController.Instance.AddToScore(UnityEngine.Random.Range(100, 1000));
                    GameSessionController.Instance.ShowPowerUpName(LocalizationController.Instance.GetWord(LocalizationFields.powerups_resetpaddle));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}