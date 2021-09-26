using Controllers.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Paddle : MonoBehaviour
    {
        // || Inspector References

        [Header("Configuration")]
        [SerializeField] private Sprite[] paddleSprites;

        // || State

        private Vector2 minMaxCoordinatesInX = Vector2.zero;
        private bool isOnImpulse = false;
        private int currentPaddleIndex = 1;
        private float moveSpeed = 20f;
        private float defaultSpeed = 0f;
        private float doubleSpeed = 0f;
        private float horizontal = 0f;

        // || Cached

        private BoxCollider2D boxCollider2D;
        private Rigidbody2D rigidBody2D;

        // || Properties

        public SpriteRenderer SpriteRenderer { get; private set; }

        private void Awake()
        {
            GetRequiredComponents();

            defaultSpeed = moveSpeed;
            doubleSpeed = (moveSpeed * 2);

            DefineStartPosition();
            DefineBounds();
        }

        private void FixedUpdate()
        {
            if (GameSessionController.Instance.ActualGameState == Enumerators.GameStates.GAMEPLAY)
            {
                Move();
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
                rigidBody2D = GetComponent<Rigidbody2D>();
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
            rigidBody2D.MovePosition((Vector2)startPosition);
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
                if (isOnImpulse && horizontal != 0)
                {
                    moveSpeed = doubleSpeed;
                }
                else if (!isOnImpulse)
                {
                    moveSpeed = defaultSpeed;
                }

                Vector2 direction = new Vector2((horizontal * moveSpeed * Time.fixedDeltaTime), 0);
                rigidBody2D.MovePosition((Vector2)transform.position + direction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

                Shooter shooter = FindObjectOfType<Shooter>();
                if (shooter != null)
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
        /// On move input
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void OnMove(InputAction.CallbackContext callbackContext) => horizontal = callbackContext.ReadValue<float>();

        /// <summary>
        /// On impulse input
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void OnImpulse(InputAction.CallbackContext callbackContext) => isOnImpulse = callbackContext.performed;
    }
}