using Controllers.Core;
using MVC.Enums;
using System;
using UnityEngine;
using Utilities;

namespace Core.PowerUps
{
    public class MoveBlocks : PowerUp
    {
        // || Inspector References

        [Header("Additional Required Configuration")]
        [SerializeField] private Enumerators.Directions direction = Enumerators.Directions.None;

        /// <summary>
        /// Applies power up effect
        /// </summary>
        protected override void Apply() => Move(direction);

        /// <summary>
        /// Move block to desired direction
        /// </summary>
        /// <param name="direction"> Desired direction </param>
        private void Move(Enumerators.Directions direction)
        {
            try
            {
                if (GameSessionController.Instance.CanMoveBlocks)
                {
                    GameSessionController.Instance.CanMoveBlocks = false;
                    GameSessionController.Instance.BlockDirection = Enumerators.Directions.None;
                }

                GameSessionController.Instance.CanMoveBlocks = true;
                GameSessionController.Instance.BlockDirection = direction;
                GameSessionController.Instance.AddToScore(UnityEngine.Random.Range(0, 1000));
                GetTranslation(direction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get translation by direction
        /// </summary>
        /// <param name="direction"> Desired direction </param>
        private static void GetTranslation(Enumerators.Directions direction)
        {
            if (direction != Enumerators.Directions.None)
            {
                LocalizationFields field = LocalizationFields.powerups_onehitblock;
                switch (direction)
                {
                    case Enumerators.Directions.Down: field = LocalizationFields.powerups_pullblocksdown; break;
                    case Enumerators.Directions.Up: field = LocalizationFields.powerups_pullblocksup; break;
                    case Enumerators.Directions.Left: field = LocalizationFields.powerups_pullblocksleft; break;
                    case Enumerators.Directions.Right: field = LocalizationFields.powerups_pullblocksright; break;
                    default: break;
                }

                GameSessionController.Instance.ShowPowerUpName(LocalizationController.Instance.GetWord(field));
            }
        }
    }
}