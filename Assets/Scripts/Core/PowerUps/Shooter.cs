using Controllers.Core;
using MVC.Enums;
using System;
using UnityEngine;

namespace Core.PowerUps
{
    public class Shooter : PowerUp
    {
        // || Inspector References

        [Header("Required Configuration")]
        [SerializeField] private Core.Shooter shooterPrefab;

        /// <summary>
        /// Applies power up effect
        /// </summary>
        protected override void Apply()
        {
            try
            {
                Core.Shooter shooter = FindAnyObjectByType<Core.Shooter>();
                if (shooter != null) return;

                shooter = Instantiate(shooterPrefab, paddle.transform.position, Quaternion.identity) as Core.Shooter;
                shooter.transform.parent = paddle.transform;
                GameSessionController.Instance.AddToScore(UnityEngine.Random.Range(100, 500));
                GameSessionController.Instance.ShowPowerUpName(LocalizationController.Instance.GetWord(LocalizationFields.powerups_shooter));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}