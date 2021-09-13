using System;
using Controllers.Core;
using Effects;
using Luminosity.IO;
using UnityEngine;
using Utilities;

namespace Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Paddle : MonoBehaviour
    {
        // || Inspector References

        [Header("Configuration")]
        [SerializeField] private Ball ball;
        [SerializeField] private EchoEffect echoEffectSpawner;
        [SerializeField] private Sprite[] paddleSprites;

        // || State

        private int currentPaddleIndex = 1;
        private float defaultSpeed = 0f;
        private float doubleSpeed = 0f;
        private Vector2 minMaxCoordinatesInX = Vector2.zero;
        private float moveSpeed = 15f;

        // || Cached

        private BoxCollider2D boxCollider2D;

        public SpriteRenderer SpriteRenderer { get; private set; }

        public Sprite GetSprite()
        {
            return SpriteRenderer.sprite;
        }

        private void Awake()
        {
            GetRequiredComponents();

            // Default values
            defaultSpeed = moveSpeed;
            doubleSpeed = (moveSpeed * 2);
            echoEffectSpawner.tag = NamesTags.PaddleEchoTag;

            DefineStartPosition();
            DefineBounds();
        }

        private void Update()
        {
            if (GameSessionController.Instance.ActualGameState == Enumerators.GameStates.GAMEPLAY)
            {
                DefineBounds();
                Move();
                CheckAndFindBall();
                LockPositionToScreen();
            }
        }

        /// <summary>
        /// Get required components
        /// </summary>
        public void GetRequiredComponents()
        {
            try
            {
                boxCollider2D = GetComponent<BoxCollider2D>();
                SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Define start position of Paddle
        /// </summary>
        private void DefineStartPosition()
        {
            Vector3 startPosition = new Vector3(Screen.width / 2f, Screen.height / 10f, 0);
            startPosition = Camera.main.ScreenToWorldPoint(startPosition);
            transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z);
        }

        /// <summary>
        /// Define bounds for Paddle
        /// </summary>
        public void DefineBounds()
        {
            float minScreenX = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
            float maxScreenX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
            float spriteExtentsX = SpriteRenderer.bounds.extents.x;
            minMaxCoordinatesInX = new Vector2(minScreenX + spriteExtentsX, maxScreenX - spriteExtentsX);
        }

        /// <summary>
        /// Move paddle by Input or Autoplay
        /// </summary>
        private void Move()
        {
            try
            {
                if (GameSessionController.Instance.IsAutoplayEnabled)
                {
                    Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    newPosition.x = ball.transform.position.x;
                    transform.position = newPosition;
                }
                else
                {
                    float horizontal = InputManager.GetAxis("Horizontal");

                    // Double the speed and shows effect
                    if (InputManager.GetButton("Impulse") && horizontal != 0)
                    {
                        moveSpeed = doubleSpeed;
                        if (echoEffectSpawner)
                        {
                            echoEffectSpawner.enabled = true;
                        }
                    }
                    else if (InputManager.GetButtonUp("Impulse"))
                    {
                        moveSpeed = defaultSpeed;
                        if (echoEffectSpawner)
                        {
                            echoEffectSpawner.enabled = false;
                        }
                    }

                    transform.Translate(horizontal * moveSpeed * Time.deltaTime, 0f, 0f);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Clamp paddle position to Screen
        /// </summary>
        private void LockPositionToScreen()
        {
            float positionInX = transform.position.x;
            positionInX = Mathf.Clamp(positionInX, minMaxCoordinatesInX.x, minMaxCoordinatesInX.y);
            transform.position = new Vector3(positionInX, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// Expands or shrink paddle
        /// </summary>
        /// <param name="toExpand"> Is to expand the paddle size ? </param>
        public void DefinePaddleSize(bool toExpand)
        {
            currentPaddleIndex = (toExpand ? currentPaddleIndex + 1 : currentPaddleIndex - 1);
            if (currentPaddleIndex < 0)
            {
                currentPaddleIndex++;
                return;
            }
            else if (currentPaddleIndex >= paddleSprites.Length)
            {
                currentPaddleIndex--;
                return;
            }

            ResetProperties();
        }

        /// <summary>
        /// Reset paddle
        /// </summary>
        public void ResetPaddle()
        {
            currentPaddleIndex = 1;
            ResetProperties();
        }

        /// <summary>
        /// Reset paddle's properties
        /// </summary>
        private void ResetProperties()
        {
            try
            {
                SpriteRenderer.sprite = paddleSprites[currentPaddleIndex];
                Destroy(boxCollider2D);
                boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
                DefineBounds();

                // Case have shooter power up
                Shooter shooter = FindObjectOfType<Shooter>();
                if (shooter)
                {
                    shooter.SetCannonsPosition();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check and find current ball
        /// </summary>
        private void CheckAndFindBall()
        {
            if (ball) return;
            ball = FindObjectOfType<Ball>();
        }
    }
}