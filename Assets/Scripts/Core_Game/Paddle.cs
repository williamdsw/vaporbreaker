using Luminosity.IO;
using UnityEngine;
using Utilities;

public class Paddle : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Ball ball;
    [SerializeField] private EchoEffect echoEffectSpawner;
    [SerializeField] private Sprite[] paddleSprites;

    private int currentPaddleIndex = 1;
    private float defaultSpeed = 0f;
    private float doubleSpeed = 0f;
    private float maxXCoordinate;
    private float minXCoordinate;
    private float moveSpeed = 15f;

    // Cached Components
    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer spriteRenderer;

    // Cached Others
    private Camera mainCamera;
    private GameSession gameSession;

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }

    private void Awake()
    {
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        rigidBody2D = this.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        gameSession = FindObjectOfType<GameSession>();

        // Default values
        defaultSpeed = moveSpeed;
        doubleSpeed = moveSpeed * 2;
        echoEffectSpawner.tag = NamesTags.PaddleEchoTag;

        DefineStartPosition();
        DefineBounds();
    }

    private void Update()
    {
        if (gameSession.GetActualGameState() == GameState.GAMEPLAY)
        {
            DefineBounds();
            Move();
            CheckAndFindBall();
        }
    }

    private void DefineStartPosition()
    {
        Vector3 startPosition = new Vector3(Screen.width / 2f, Screen.height / 10f, 0);
        startPosition = mainCamera.ScreenToWorldPoint(startPosition);
        transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z);
    }

    public void DefineBounds()
    {
        if (!mainCamera || !spriteRenderer) return;

        float minScreenX = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        float maxScreenX = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        float spriteExtentsX = spriteRenderer.bounds.extents.x;
        minXCoordinate = minScreenX + spriteExtentsX;
        maxXCoordinate = maxScreenX - spriteExtentsX;
    }

    private void LockPositionToScreen()
    {
        float xPosition = transform.position.x;
        xPosition = Mathf.Clamp(xPosition, minXCoordinate, maxXCoordinate);
        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
    }

    private void Move()
    {
        if (gameSession.GetIsAutoplayEnabled())
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

        LockPositionToScreen();
    }

    // Expands or shrink paddle size if index is valid
    public void DefinePaddleSize(bool toExpand)
    {
        // Check index
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

        // Define properties
        spriteRenderer.sprite = paddleSprites[currentPaddleIndex];
        Destroy(boxCollider2D);
        boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();
        DefineBounds();

        // Case have shooter power up
        Shooter shooter = FindObjectOfType<Shooter>();
        if (shooter)
        {
            shooter.DefineCannonsPosition();
        }
    }

    // Resets the paddle
    public void ResetPaddle()
    {
        currentPaddleIndex = 1;

        // Define properties
        spriteRenderer.sprite = paddleSprites[currentPaddleIndex];
        Destroy(boxCollider2D);
        boxCollider2D = this.gameObject.AddComponent<BoxCollider2D>();
        DefineBounds();

        // Case have shooter power up
        Shooter shooter = FindObjectOfType<Shooter>();
        if (shooter)
        {
            shooter.DefineCannonsPosition();
        }
    }

    private void CheckAndFindBall()
    {
        if (ball) return;
        ball = FindObjectOfType<Ball>();
    }
}