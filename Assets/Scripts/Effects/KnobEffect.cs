using System;
using UnityEngine;

namespace Effects
{
    /// <summary>
    /// Effect for TV Knob
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class KnobEffect : MonoBehaviour
    {
        // || Cached

        private Animator animator;

        // || Properties

        public static KnobEffect Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            GetRequiredComponents();
        }

        /// <summary>
        /// Get required components
        /// </summary>
        private void GetRequiredComponents()
        {
            try
            {
                animator = GetComponent<Animator>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Play turn diretion animation
        /// </summary>
        /// <param name="triggerName"> Name of the trigger </param>
        public void TurnDirection(string triggerName) => animator.SetTrigger(triggerName);
    }
}