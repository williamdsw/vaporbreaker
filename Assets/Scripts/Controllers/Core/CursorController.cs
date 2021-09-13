using Controllers.Menu;
using Luminosity.IO;
using System;
using UnityEngine;
using Utilities;

namespace Controllers.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class CursorController : MonoBehaviour
    {
        // || State

        [SerializeField] private Vector2 minXYCoordinates = Vector2.zero;
        [SerializeField] private Vector2 maxXYCoordinates = Vector2.zero;


        private float speed = 100f;
        private Vector3 startPosition;

        // || Cached

        private SpriteRenderer spriteRenderer;

        // || Properties

        public static CursorController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            GetRequiredComponents();
            ConfigurationsController.ToggleCursor(true);
            DefineBounds();
        }

        private void Update()
        {
            if (GameSessionController.Instance.ActualGameState == Enumerators.GameStates.GAMEPLAY)
            {
                DefineBounds();
                SetPosition();
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
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Define cursor bounds
        /// </summary>
        private void DefineBounds()
        {
            try
            {
                Vector3 zeroPoints = new Vector3(0, 0, 0);
                Vector3 screenSize = new Vector3(Screen.width, Screen.height, 0);
                float minScreenX = Camera.main.ScreenToWorldPoint(zeroPoints).x;
                float maxScreenX = Camera.main.ScreenToWorldPoint(screenSize).x;
                float minScreenY = Camera.main.ScreenToWorldPoint(zeroPoints).y;
                float maxScreenY = Camera.main.ScreenToWorldPoint(screenSize).y;
                float spriteExtentsX = spriteRenderer.bounds.extents.x;
                float spriteExtentsY = spriteRenderer.bounds.extents.y;

                // Set
                minXYCoordinates.x = (minScreenX + spriteExtentsX);
                maxXYCoordinates.x = (maxScreenX - spriteExtentsX);
                minXYCoordinates.y = (minScreenY + spriteExtentsY);
                maxXYCoordinates.y = (maxScreenY - spriteExtentsY);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Set cursor position by mouse or gamepad
        /// </summary>
        private void SetPosition()
        {
            if (GamepadState.IsConnected(GamepadIndex.GamepadOne))
            {
                Vector3 inputDirection = Vector3.zero;
                float horizontal = InputManager.GetAxis("MouseHorizontal");
                float vertical = InputManager.GetAxis("MouseVertical");
                inputDirection.x = (horizontal * speed * Time.deltaTime);
                inputDirection.y = (vertical * speed * Time.deltaTime);
                transform.position = (startPosition + inputDirection * 0.2f);
                startPosition = transform.position;
            }
            else
            {
                Camera mainCamera = Camera.main;
                if (!mainCamera)
                {
                    mainCamera = FindObjectOfType<Camera>();
                    return;
                }

                Vector2 cursorPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                transform.position = cursorPosition;
            }
        }

        /// <summary>
        /// Lock cursor position to screen
        /// </summary>
        private void LockPositionToScreen()
        {
            float positionInX = transform.position.x;
            float positionInY = transform.position.y;
            positionInX = Mathf.Clamp(positionInX, minXYCoordinates.x, maxXYCoordinates.x);
            positionInY = Mathf.Clamp(positionInY, minXYCoordinates.y, maxXYCoordinates.y);
            transform.position = new Vector3(positionInX, positionInY, transform.position.z);
        }

        /// <summary>
        /// Destroy object
        /// </summary>
        public void DestroyInstance() => Destroy(gameObject);
    }
}