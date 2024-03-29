using Controllers.Core;
using MVC.Enums;
using System;

namespace Core.PowerUps
{
    public class AllBlocksOneHit : PowerUp
    {
        /// <summary>
        /// Applies power up effect
        /// </summary>
        protected override void Apply()
        {
            try
            {
                Block[] blocks = FindObjectsOfType<Block>();
                if (blocks.Length != 0)
                {
                    foreach (Block block in blocks)
                    {
                        block.MaxHits = block.StartMaxHits = 1;
                    }

                    GameSessionController.Instance.AddToScore(UnityEngine.Random.Range(0, 1000));
                    GameSessionController.Instance.ShowPowerUpName(LocalizationController.Instance.GetWord(LocalizationFields.powerups_onehitblock));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}