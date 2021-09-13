using System;
using Controllers.Core;
using Core;
using UnityEngine;
using Utilities;

namespace Effects
{
    public class EchoEffect : MonoBehaviour
    {
        // || Inspector References

        [Header("Required Configuration")]
        [SerializeField] private GameObject echoPrefab;

        // || Config

        private float startTimeBetweenSpanws = 0.05f;

        // || State

        private float timeBetweenSpawns = 0;

        // || Cached

        private Ball ball;
        private Paddle paddle;

        // || Properties

        public float TimeToSelfDestruct { get; set; } = 1f;

        private void Awake() => FindNeededReferences();

        private void Update() => SpawnEchoEffect();

        /// <summary>
        /// Find needed references
        /// </summary>
        private void FindNeededReferences()
        {
            try
            {
                if (tag == NamesTags.BallEchoTag)
                {
                    ball = transform.parent.GetComponent<Ball>();
                }
                else if (tag == NamesTags.PaddleEchoTag)
                {
                    paddle = transform.parent.GetComponent<Paddle>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Spawn echo effect
        /// </summary>
        private void SpawnEchoEffect()
        {
            try
            {
                if (GameSessionController.Instance.ActualGameState != Enumerators.GameStates.GAMEPLAY) return;

                if (timeBetweenSpawns <= 0)
                {
                    GameObject echo = Instantiate(echoPrefab, transform.position, Quaternion.identity) as GameObject;
                    echo.transform.parent = GameSessionController.Instance.FindOrCreateObjectParent(NamesTags.EchosParentName).transform;
                    if (tag.Equals(NamesTags.BallEchoTag) && ball)
                    {
                        echo.transform.localScale = ball.transform.localScale;
                        echo.transform.rotation = ball.transform.rotation;
                        SpriteRenderer spriteRenderer = echo.GetComponent<SpriteRenderer>();
                        spriteRenderer.color = ball.CurrentColor;
                        spriteRenderer.sprite = ball.Sprite;
                    }
                    else if (tag.Equals(NamesTags.PaddleEchoTag) && paddle)
                    {
                        SpriteRenderer spriteRenderer = echo.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = paddle.GetSprite();
                    }

                    Destroy(echo, TimeToSelfDestruct);
                    timeBetweenSpawns = startTimeBetweenSpanws;
                }
                else
                {
                    timeBetweenSpawns -= Time.deltaTime;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}