using Controllers.Core;
using Luminosity.IO;
using MVC.Global;
using UnityEngine;
using Utilities;

public class Ball : MonoBehaviour
{
    [Header("Configuration Parameters")]
    [SerializeField] private EchoEffect echoEffectSpawnerPrefab;
    [SerializeField] private GameObject initialLinePrefab;
    [SerializeField] private GameObject paddleParticlesPrefab;
    [SerializeField] private Paddle paddle;

    // Speed config
    private float defaultSpeed = 400f;
    private float maxMoveSpeed = 1000f;
    private float minMoveSpeed = 200f;
    private float moveSpeed = 400f;

    // Scale config
    private float maxBallLocalScale = 8f;
    private float minBallLocalScale = 0.5f;

    // Rotation config
    private float ballRotationDegree = 20f;
    private float maxBallRotationDegree = 90f;
    private float minBallRotationDegree = 10f;

    // State
    private Vector3 paddleToBallPosition;
    private Vector3 remainingPosition;
    private bool isBallFree = false;

    // Cached Components
    private LineRenderer initialLineRenderer;
    private Rigidbody2D rigidBody2D;
    private SpriteRenderer spriteRenderer;

    // Cached Other Objects
    private AudioController audioController;
    private Camera mainCamera;
    private CursorController cursorController;
    private GameSession gameSession;

    public Color GetBallColor()
    {
        return spriteRenderer.color;
    }

    public float GetBallRotationDegree()
    {
        return ballRotationDegree;
    }

    public float GetDefaultSpeed()
    {
        return defaultSpeed;
    }

    public float GetMaxBallLocalScale()
    {
        return maxBallLocalScale;
    }

    public float GetMaxBallRotationDegree()
    {
        return maxBallRotationDegree;
    }

    public float GetMaxMoveSpeed()
    {
        return maxMoveSpeed;
    }

    public float GetMinBallLocalScale()
    {
        return minBallLocalScale;
    }

    public float GetMinBallRotationDegree()
    {
        return minBallRotationDegree;
    }

    public float GetMinMoveSpeed()
    {
        return minMoveSpeed;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public bool GetIsBallFree()
    {
        return this.isBallFree;
    }

    public void SetBallRotationDegree(float ballRotationDegree)
    {
        this.ballRotationDegree = ballRotationDegree;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    public void SetIsBallFree(bool isBallFree)
    {
        this.isBallFree = isBallFree;
    }

    private void Awake() 
    {
        rigidBody2D = this.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (!initialLinePrefab)
        {
            initialLinePrefab = GameObject.FindGameObjectWithTag(NamesTags.LineBetweenBallPointerTag);
        }

        initialLineRenderer = initialLinePrefab.GetComponent<LineRenderer>();

        // Other Objects
        audioController = FindObjectOfType<AudioController>();
        cursorController = FindObjectOfType<CursorController>();
        gameSession = FindObjectOfType<GameSession>();

        // Default values
        mainCamera = Camera.main;
        defaultSpeed = moveSpeed;
        echoEffectSpawnerPrefab.tag = NamesTags.BallEchoTag;

        if (!isBallFree)
        {
            if (!paddle)
            {
                paddle = FindObjectOfType<Paddle>();
            }

            paddleToBallPosition = this.transform.position - paddle.transform.position;
            this.transform.position = new Vector3(paddle.transform.position.x, paddle.transform.position.y + 0.25f, paddle.transform.position.z);

            DrawLineToMouse();
        }

        if (FindObjectsOfType<Ball>().Length > 1)
        {
            ChooseRandomColor();
        }
    }

    private void Update()
    {
        if (gameSession.GetActualGameState() == GameState.GAMEPLAY)
        {
            if (!gameSession.GetHasStarted())
            {
                if (FindObjectsOfType<Ball>().Length == 1)
                {
                    LockBallToPaddle();
                    CalculateDistanceToMouse();
                    DrawLineToMouse();
                    LaunchOnPointerClick();
                }
            }
            else
            {
                if (isBallFree)
                {
                    RotateBall();
                    if (rigidBody2D.velocity == Vector2.zero) { ClampVelocity(); }
                }
                else
                {
                    LockBallToPaddle();
                    CalculateDistanceToMouse();
                    DrawLineToMouse();
                    LaunchOnPointerClick();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (gameSession.GetActualGameState() == GameState.GAMEPLAY)
        {
            if (gameSession.GetHasStarted())
            {
                bool isBallBig = (this.transform.localScale.y >= 8f);

                // Combo manipulator
                if (other.gameObject.CompareTag(NamesTags.PaddleTag) || other.gameObject.CompareTag(NamesTags.WallTag))
                {
                    gameSession.ResetCombo();
                }

                switch (other.gameObject.tag)
                {
                    // Colision with paddle
                    case "Paddle":
                    {
                        if (other.GetContact(0).normal != Vector2.down)
                        {
                            // Increments direction
                            float hitFactor = CalculateHitFactor(this.transform.position, other.transform.position, other.collider.bounds.size.x);
                            Vector2 hitDirection = new Vector2(hitFactor * 2f, 0f);
                            hitDirection *= Vector2.one;
                            hitDirection.Normalize();
                            Vector2 velocity = rigidBody2D.velocity;
                            velocity += hitDirection;
                            rigidBody2D.velocity = velocity.normalized * moveSpeed * Time.deltaTime;
                            AudioClip clip = (isBallBig ? audioController.HittingFace : audioController.BlipSound);
                            audioController.PlaySFX(clip, audioController.MaxSFXVolume);
                        }

                        if (other.GetContact(0).normal == Vector2.up)
                        {
                            if (moveSpeed > defaultSpeed)
                            {
                                SpawnPaddleDebris(other.GetContact(0).point);
                            }
                        }

                        break;
                    }

                    // Colision with walls
                    case "Wall":
                    {
                        ClampVelocity();
                        AudioClip clip = (isBallBig ? audioController.HittingFace : audioController.BlipSound);
                        audioController.PlaySFX(clip, audioController.MaxSFXVolume);
                        break;
                    }

                    case "Breakable":
                    case "Unbreakable":
                    {
                        Vector2 normal = other.GetContact(0).normal;
                        Vector2 direction = this.transform.forward;
                        direction = Vector2.Reflect(direction, normal);
                        Vector2 velocity = rigidBody2D.velocity;
                        velocity += direction;
                        rigidBody2D.velocity = velocity.normalized * moveSpeed * Time.deltaTime;
                        break;
                    }

                    default: break;
                }
            }
        }
    }

    // Calculates hit factor on paddle
    private float CalculateHitFactor(Vector2 ballPosition, Vector2 paddlePosition, float paddleWidth)
    {
        return (ballPosition.x - paddlePosition.x) / paddleWidth;
    }

    // Locks the ball
    private void LockBallToPaddle()
    {
        Vector3 paddlePosition = new Vector3(paddle.transform.position.x, paddle.transform.position.y, 0f);
        this.transform.position = new Vector3(paddlePosition.x + paddleToBallPosition.x, paddlePosition.y + 0.35f, transform.position.z);
    }

    // Calculate distance to pointer (mouse)
    private void CalculateDistanceToMouse()
    {
        //Vector3 mousePosition = mainCamera.ScreenToWorldPoint (Input.mousePosition);
        Vector3 mousePosition = cursorController.transform.position;
        remainingPosition = mousePosition - this.transform.position;
        remainingPosition.z = 0f;
    }

    // 
    private void LaunchOnPointerClick()
    {
        if (remainingPosition.y >= 1f)
        {
            if (InputManager.GetButtonDown(Configuration.InputsNames.Shoot))
            {
                gameSession.SetHasStarted(true);
                float startTimeToSpawnAnotherBall = Random.Range(5f, 20f);
                gameSession.SetStartTimeToSpawnAnotherBall(startTimeToSpawnAnotherBall);
                gameSession.SetCanSpawnAnotherBall(true);
                gameSession.CurrentNumberOfLaunchedBalls++;
                remainingPosition.Normalize();
                rigidBody2D.velocity = (remainingPosition * moveSpeed * Time.deltaTime);
                initialLineRenderer.enabled = false;
                isBallFree = true;
            }
        }
    }

    //--------------------------------------------------------------------------------//
    // LINE RENDERERS

    // Draws a line beetween the ball and mouse
    private void DrawLineToMouse()
    {
        if (!initialLineRenderer.enabled)
        {
            initialLineRenderer.enabled = true;
        }

        // Positions
        Vector3 pointerPosition = cursorController.transform.position;
        Vector3 ballPosition = this.transform.position;
        ballPosition = new Vector3(ballPosition.x, ballPosition.y + 0.2f, ballPosition.z);
        initialLineRenderer.SetPositions(new Vector3[] { ballPosition, pointerPosition });
    }

    //--------------------------------------------------------------------------------//

    // COLLISION VELOCITY ISSUES

    private void ClampVelocity()
    {
        Vector2 currentVelocity = rigidBody2D.velocity;
        float x = Mathf.Clamp(Mathf.Abs(currentVelocity.x), 2f, 20f);
        float y = Mathf.Clamp(Mathf.Abs(currentVelocity.y), 2f, 20f);
        currentVelocity.x = (currentVelocity.x > 0 ? x : x * -1);
        currentVelocity.y = (currentVelocity.y > 0 ? y : y * -1);
        rigidBody2D.velocity = currentVelocity;
    }

    private void RotateBall()
    {
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.z += ballRotationDegree;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    //--------------------------------------------------------------------------------//
    // VFX

    // Chooses an random color
    public void ChooseRandomColor()
    {
        if (!spriteRenderer) return;
        Color randomColor = Random.ColorHSV(0f, 1f, 0f, 1f, 0.4f, 1f);
        spriteRenderer.color = randomColor;
    }

    // Spawns debris on paddle collision
    private void SpawnPaddleDebris(Vector2 contactPoint)
    {
        if (paddleParticlesPrefab)
        {
            // Instantiate and Destroy
            GameObject particles = Instantiate(paddleParticlesPrefab, contactPoint, paddleParticlesPrefab.transform.rotation) as GameObject;
            particles.transform.SetParent(gameSession.FindOrCreateObjectParent(NamesTags.DebrisParentName).transform);
            ParticleSystem debrisParticleSystem = paddleParticlesPrefab.GetComponent<ParticleSystem>();
            float durationLength = debrisParticleSystem.main.duration + debrisParticleSystem.main.startLifetime.constant;
            Destroy(particles, durationLength);
        }
    }
}