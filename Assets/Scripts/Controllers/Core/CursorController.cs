using Controllers.Panel;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Controllers.Core
{
    /// <summary>
    /// Controller for In-Game Cursor
    /// </summary>
    public class CursorController : MonoBehaviour
    {
        // || State

        [SerializeField] private Vector2 minXYCoordinates = Vector2.zero;
        [SerializeField] private Vector2 maxXYCoordinates = Vector2.zero;

        // || Properties

        public static CursorController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            ConfigurationsController.ToggleCursor(false);
            DefineBounds();
        }

        private void Update()
        {
            if (GameSessionController.Instance.ActualGameState == Enumerators.GameStates.GAMEPLAY)
            {
                SetPosition();
                LockPositionToScreen();
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

                minXYCoordinates = new Vector2(minScreenX, minScreenY);
                maxXYCoordinates = new Vector2(maxScreenX, maxScreenY);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Set cursor position by mouse or gamepad
        /// </summary>
        private void SetPosition() => transform.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

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
    }
}