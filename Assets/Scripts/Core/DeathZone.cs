using Controllers.Core;
using Effects;
using System;
using System.Collections;
using UnityEngine;
using Utilities;

namespace Core
{
    [RequireComponent(typeof(EdgeCollider2D))]
    public class DeathZone : MonoBehaviour
    {
        // || Cached

        private EdgeCollider2D edgeCollider;
        private Ball[] balls;

        private void Awake() => GetRequiredComponents();

        private void Start() => DefineColliderPoints();

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (GameSessionController.Instance.ActualGameState == Enumerators.GameStates.GAMEPLAY)
            {
                if (other.gameObject.CompareTag(NamesTags.Tags.Ball))
                {
                    DealWithBallCollision(other.gameObject);
                }
                else if (other.gameObject.CompareTag(NamesTags.Tags.PowerUp))
                {
                    Destroy(other.gameObject);
                }
            }
        }

        /// <summary>
        /// Get required components
        /// </summary>
        public void GetRequiredComponents()
        {
            try
            {
                edgeCollider = GetComponent<EdgeCollider2D>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Set collider points with Screen size
        /// </summary>
        private void DefineColliderPoints()
        {
            try
            {
                Vector2 lowerLeftCorner = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
                Vector2 lowerRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
                lowerLeftCorner.x = Mathf.FloorToInt(lowerLeftCorner.x) * 2;
                lowerRightCorner.x = Mathf.CeilToInt(lowerRightCorner.x) * 2;
                edgeCollider.points = new Vector2[] { lowerLeftCorner, lowerRightCorner };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deal with ball collision
        /// </summary>
        /// <param name="otherBall"> Other ball to compare </param>
        private void DealWithBallCollision(GameObject otherBall)
        {
            try
            {
                balls = FindObjectsOfType<Ball>();

                if (balls.Length == 1)
                {
                    StartCoroutine(WaitToReset());
                }
                else
                {
                    AudioController.Instance.PlaySFX(AudioController.Instance.BoomSound, AudioController.Instance.MaxSFXVolume / 2);
                    GameSessionController.Instance.CurrentNumberOfBalls--;
                }

                Destroy(otherBall);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Wait to reset the game
        /// </summary>
        private IEnumerator WaitToReset()
        {
            GameSessionController.Instance.ActualGameState = Enumerators.GameStates.TRANSITION;
            AudioController.Instance.PlaySFX(AudioController.Instance.BoomSound, AudioController.Instance.MaxSFXVolume);
            yield return new WaitForSecondsRealtime(AudioController.Instance.GetClipLength(AudioController.Instance.BoomSound));
            FadeEffect.Instance.FadeToLevel();
        }
    }
}