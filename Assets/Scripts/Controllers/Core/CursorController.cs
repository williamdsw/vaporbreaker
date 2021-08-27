using Luminosity.IO;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private float minXCoordinate;
    private float maxXCoordinate;
    private float minYCoordinate;
    private float maxYCoordinate;
    private float speed = 100f;
    private Vector3 startPosition;

    // Cached
    private SpriteRenderer spriteRenderer;

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    private void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        Cursor.visible = false;
        DefineBounds();
    }

    private void Update()
    {
        FindAndSetPosition();
        DefineBounds();
        LockPositionToScreen();
    }

    private void SetupSingleton()
    {
        int numberOfInstances = FindObjectsOfType(GetType()).Length;
        if (numberOfInstances > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Sets position to either mouse or gamepad
    private void FindAndSetPosition()
    {
        if (GamepadState.IsConnected(GamepadIndex.GamepadOne))
        {
            Vector3 inputDirection = Vector3.zero;
            float horizontal = InputManager.GetAxis("MouseHorizontal");
            float vertical = InputManager.GetAxis("MouseVertical");
            inputDirection.x = horizontal * speed * Time.deltaTime;
            inputDirection.y = vertical * speed * Time.deltaTime;
            transform.position = startPosition + inputDirection * 0.2f;
            startPosition = transform.position;
        }
        else
        {
            // Finds camera
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

    // Define bounds to camera
    private void DefineBounds()
    {
        // Cancels
        if (!spriteRenderer) return;

        // Values
        Vector3 zeroPoints = new Vector3(0, 0, 0);
        Vector3 screenSize = new Vector3(Screen.width, Screen.height, 0);
        float minScreenX = Camera.main.ScreenToWorldPoint(zeroPoints).x;
        float maxScreenX = Camera.main.ScreenToWorldPoint(screenSize).x;
        float minScreenY = Camera.main.ScreenToWorldPoint(zeroPoints).y;
        float maxScreenY = Camera.main.ScreenToWorldPoint(screenSize).y;
        float spriteExtentsX = spriteRenderer.bounds.extents.x;
        float spriteExtentsY = spriteRenderer.bounds.extents.y;

        // Set
        minXCoordinate = minScreenX + spriteExtentsX;
        maxXCoordinate = maxScreenX - spriteExtentsX;
        minYCoordinate = minScreenY + spriteExtentsY;
        maxYCoordinate = maxScreenY - spriteExtentsY;
    }

    private void LockPositionToScreen()
    {
        float xPosition = transform.position.x;
        float yPosition = transform.position.y;
        xPosition = Mathf.Clamp(xPosition, minXCoordinate, maxXCoordinate);
        yPosition = Mathf.Clamp(yPosition, minYCoordinate, maxYCoordinate);
        transform.position = new Vector3(xPosition, yPosition, transform.position.z);
    }

    public void DestroyInstance()
    {
        Destroy(this.gameObject);
    }
}