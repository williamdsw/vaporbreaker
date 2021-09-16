using Controllers.Core;
using MVC.Enums;
using System;
using UnityEngine;
using Utilities;

namespace Core.PowerUps
{
    public class UnbreakablesToBreakables : PowerUp
    {
        /// <summary>
        /// Applies power up effect
        /// </summary>
        protected override void Apply()
        {
            try
            {
                if (GameObject.FindGameObjectsWithTag(NamesTags.UnbreakableBlockTag).Length != 0)
                {
                    GameObject[] unbreakables = GameObject.FindGameObjectsWithTag(NamesTags.UnbreakableBlockTag);
                    if (unbreakables.Length != 0)
                    {
                        foreach (GameObject unbreakable in unbreakables)
                        {
                            unbreakable.tag = NamesTags.BreakableBlockTag;
                            GameSessionController.Instance.CountBlocks();
                            unbreakable.GetComponent<Animator>().enabled = false;

                            foreach (Transform child in unbreakable.transform)
                            {
                                Destroy(child.gameObject);
                            }
                        }

                        GameSessionController.Instance.AddToScore(UnityEngine.Random.Range(100, 500));
                        GameSessionController.Instance.ShowPowerUpName(LocalizationController.Instance.GetWord(LocalizationFields.loading_unbreakableblock));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}