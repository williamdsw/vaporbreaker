using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Config
    [SerializeField] private Shooter shooterPrefab;

    // Rotation
    private float angleToIncrement = 0f;
    private int canRotateChance;

    // Speed
    private float moveSpeed = 0f;
    private float minMoveSpeed = 10f;
    private float maxMoveSpeed = 30f;

    //Force 
    private float minForceX = -1000f;
    private float maxForceX = 1000f;
    private float minForceY = 0f;
    private float maxForceY = 1000f;

    // State
    private int currentPowerUpIndex = 0;
    private string[] powerUpNames =
    {
        NamesTags.GetPowerUpAllBlocks1HitName (),
        NamesTags.GetPowerUpBallBiggerName (),
        NamesTags.GetPowerUpBallSmallerName (),
        NamesTags.GetPowerUpBallFasterName (),
        NamesTags.GetPowerUpBallSlowerName (),
        NamesTags.GetPowerUpDuplicateBallName (),
        NamesTags.GetPowerUpPaddleExpandName (),
        NamesTags.GetPowerUpPaddleShrinkName (),
        NamesTags.GetPowerUpLevelCompleteName (),
        NamesTags.GetPowerUpResetBallName (),
        NamesTags.GetPowerUpResetPaddleName (),
        NamesTags.GetPowerUpShooterName (),
        NamesTags.GetPowerUpUnbreakablesToBreakablesName (),
        NamesTags.GetPowerUpZeroDeathsName ()
    };

    // Cached Components
    private Rigidbody2D rigidBody2D;

    // Cached Others
    private AudioController audioController;
    private GameSession gameSession;
    private LocalizationController localizationController;
    private Paddle paddle;

    private void Awake()
    {
        rigidBody2D = this.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Find other
        audioController = FindObjectOfType<AudioController>();
        gameSession = FindObjectOfType<GameSession>();
        localizationController = FindObjectOfType<LocalizationController>();
        paddle = FindObjectOfType<Paddle>();

        // Random values
        angleToIncrement = Random.Range(0f, 16f);
        canRotateChance = Random.Range(0, 100);

        AddRandomForce();
    }

    private void Update()
    {
        // Rotates... or not
        if (gameSession.GetActualGameState() == GameState.GAMEPLAY)
        {
            if (canRotateChance >= 50)
            {
                RotateObject();
            }
        }
    }

    // Collision with paddle
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (gameSession.GetActualGameState() == GameState.GAMEPLAY)
        {
            Paddle paddle = other.collider.GetComponent<Paddle>();
            if (paddle)
            {
                DealCollisionWithPaddle();
            }
        }
    }

    private void AddRandomForce()
    {
        float randomX = Random.Range(minForceX, maxForceX);
        float randomY = Random.Range(minForceY, maxForceY);
        Vector2 randomForce = new Vector2(randomX, randomY);
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed + 1);
        randomForce *= Time.deltaTime * moveSpeed;
        rigidBody2D.AddForce(randomForce);
    }

    private void RotateObject()
    {
        if (angleToIncrement != 0)
        {
            Vector3 eulerAngles = transform.rotation.eulerAngles;
            eulerAngles.y += angleToIncrement;
            transform.rotation = Quaternion.Euler(eulerAngles);
        }
    }

    private void DealCollisionWithPaddle()
    {
        Destroy(this.gameObject);
        audioController.PlaySFX(audioController.PowerUpSound, audioController.GetMaxSFXVolume());
        gameSession.SetHasPowerUpCollidedWithPaddle(true);
        gameSession.ResetHideTime();
        string name = this.gameObject.name;
        name = name.Replace("(Clone)", "");
        ApplyPowerUpEffect(name);
        SetPowerUpNameToGameSession(name);
        AddScoreToGameSession(name);
    }

    // Applies power up effect to gameplay
    private void ApplyPowerUpEffect(string powerUpName)
    {
        switch (powerUpName)
        {
            case "PowerUp_All_Blocks_1_Hit":
                {
                    MakeAllBlocksOneHit();
                    currentPowerUpIndex = 0;
                    break;
                }

            case "PowerUp_Ball_Bigger":
                {
                    DefineBallsSize(true);
                    currentPowerUpIndex = 1;
                    break;
                }

            case "PowerUp_Ball_Smaller":
                {
                    DefineBallsSize(false);
                    currentPowerUpIndex = 2;
                    break;
                }

            case "PowerUp_Ball_Faster":
                {
                    DefineBallsSpeed(true);
                    currentPowerUpIndex = 3;
                    break;
                }

            case "PowerUp_Ball_Slower":
                {
                    DefineBallsSpeed(false);
                    currentPowerUpIndex = 4;
                    break;
                }

            case "PowerUp_Duplicate_Ball":
                {
                    DuplicateBalls();
                    currentPowerUpIndex = 5;
                    break;
                }

            case "PowerUp_Paddle_Expand":
                {
                    if (paddle)
                    {
                        paddle.DefinePaddleSize(true);
                        currentPowerUpIndex = 6;
                    }

                    break;
                }

            case "PowerUp_Paddle_Shrink":
                {
                    if (paddle)
                    {
                        paddle.DefinePaddleSize(false);
                        currentPowerUpIndex = 7;
                    }
                    break;
                }

            case "PowerUp_Level_Complete":
                {
                    gameSession.CallLevelComplete();
                    currentPowerUpIndex = 8;
                    break;
                }

            case "PowerUp_Random":
                {
                    ApplyPowerUpEffect(powerUpNames[Random.Range(0, powerUpNames.Length)]);
                    break;
                }

            case "PowerUp_Reset_Ball":
                {
                    ResetBalls();
                    currentPowerUpIndex = 9;
                    break;
                }

            case "PowerUp_Reset_Paddle":
                {
                    if (paddle)
                    {
                        paddle.ResetPaddle();
                        currentPowerUpIndex = 10;
                    }

                    break;
                }

            case "PowerUp_Shooter":
                {
                    CreateShooter();
                    currentPowerUpIndex = 11;
                    break;
                }

            case "PowerUp_Unbreakables_To_Breakables":
                {
                    MakeUnbreakableToBreakable();
                    currentPowerUpIndex = 12;
                    break;
                }

            case "PowerUp_Zero_Deaths":
                {
                    gameSession.ZeroNumberOfDeaths();
                    currentPowerUpIndex = 13;
                    break;
                }

            default: break;
        }
    }

    private void SetPowerUpNameToGameSession(string powerUpName)
    {
        if (powerUpName.Equals("PowerUp_Random"))
        {
            SetPowerUpNameToGameSession(powerUpNames[currentPowerUpIndex]);
        }
        else
        {
            string name = localizationController.GetPowerUpsNames()[currentPowerUpIndex];
            gameSession.SetPowerUpName(name);
        }
    }

    private void AddScoreToGameSession(string powerUpName)
    {
        switch (powerUpName)
        {
            case "PowerUp_All_Blocks_1_Hit": { gameSession.AddToStore(Random.Range(0, 1000)); break; }
            case "PowerUp_Ball_Bigger": { gameSession.AddToStore(Random.Range(0, 1000)); break; }
            case "PowerUp_Ball_Smaller": { gameSession.AddToStore(Random.Range(1000, 5000)); break; }
            case "PowerUp_Ball_Faster": { gameSession.AddToStore(Random.Range(5000, 10000)); break; }
            case "PowerUp_Ball_Slower": { gameSession.AddToStore(Random.Range(100, 1000)); break; }
            case "PowerUp_Duplicate_Ball": { gameSession.AddToStore(Random.Range(500, 2500)); break; }
            case "PowerUp_Paddle_Expand": { gameSession.AddToStore(Random.Range(100, 500)); break; }
            case "PowerUp_Paddle_Shrink": { gameSession.AddToStore(Random.Range(10000, 30000)); break; }
            case "PowerUp_Level_Complete": { gameSession.AddToStore(Random.Range(-1000, -10000)); break; }
            case "PowerUp_Reset_Ball": { gameSession.AddToStore(Random.Range(100, 1000)); break; }
            case "PowerUp_Reset_Paddle": { gameSession.AddToStore(Random.Range(100, 1000)); break; }
            case "PowerUp_Shooter": { gameSession.AddToStore(Random.Range(100, 500)); break; }
            case "PowerUp_Unbreakables_To_Breakables": { gameSession.AddToStore(Random.Range(100, 500)); break; }
            case "PowerUp_Zero_Deaths": { gameSession.AddToStore(Random.Range(0, -1000)); break; }
            default: { break; }
        }
    }

    // Finds all blocks and make them take 1 hit
    private void MakeAllBlocksOneHit()
    {
        Block[] blocks = FindObjectsOfType<Block>();

        if (blocks.Length != 0)
        {
            foreach (Block block in blocks)
            {
                block.SetMaxHits(1);
            }
        }
    }

    // Makes the ball bigger or smaller
    private void DefineBallsSize(bool makeBigger)
    {
        Ball[] balls = FindObjectsOfType<Ball>();
        if (balls.Length != 0)
        {
            foreach (Ball ball in balls)
            {
                if (ball.GetIsBallFree())
                {
                    Vector3 newLocalScale = ball.transform.localScale;
                    if (makeBigger)
                    {
                        if (newLocalScale.x < ball.GetMaxBallLocalScale())
                        {
                            newLocalScale *= 2f;
                        }
                    }
                    else
                    {
                        if (newLocalScale.x > ball.GetMinBallLocalScale())
                        {
                            newLocalScale /= 2f;
                        }
                    }

                    ball.transform.localScale = newLocalScale;
                }
            }
        }
    }

    // Makes the ball faster or slower
    private void DefineBallsSpeed(bool moveFaster)
    {
        Ball[] balls = FindObjectsOfType<Ball>();
        if (balls.Length != 0)
        {
            foreach (Ball ball in balls)
            {
                if (ball.GetIsBallFree())
                {
                    // Speed And Rotation to add ...
                    Rigidbody2D ballRB = ball.GetComponent<Rigidbody2D>();
                    float moveSpeed = ball.GetMoveSpeed();
                    float ballRotationDegree = ball.GetBallRotationDegree();
                    if (moveFaster)
                    {
                        if (moveSpeed < ball.GetMaxMoveSpeed())
                        {
                            moveSpeed += 100f;
                        }

                        if (ballRotationDegree < ball.GetMaxBallRotationDegree())
                        {
                            ballRotationDegree *= 2;
                        }
                    }
                    else
                    {
                        if (moveSpeed > ball.GetMinMoveSpeed())
                        {
                            moveSpeed -= 100f;
                        }

                        if (ballRotationDegree > ball.GetMinBallRotationDegree())
                        {
                            ballRotationDegree /= 2;
                        }
                    }

                    ball.SetMoveSpeed(moveSpeed);
                    ball.SetBallRotationDegree(ballRotationDegree);
                    ballRB.velocity = (ballRB.velocity.normalized * Time.deltaTime * moveSpeed);
                }
            }
        }
    }

    private void ResetBalls()
    {
        Ball[] balls = FindObjectsOfType<Ball>();
        if (balls.Length != 0)
        {
            foreach (Ball ball in balls)
            {
                if (ball.GetIsBallFree())
                {
                    // Local Scale
                    ball.transform.localScale = Vector3.one;

                    // Movement
                    Rigidbody2D ballRB = ball.GetComponent<Rigidbody2D>();
                    float defaultSpeed = ball.GetDefaultSpeed();
                    ball.SetMoveSpeed(defaultSpeed);
                    ballRB.velocity = (ballRB.velocity.normalized * Time.deltaTime * defaultSpeed);
                }
            }
        }
    }

    // Duplicates que quantity of current balls in the reversed direction of each ball
    private void DuplicateBalls()
    {
        Ball[] balls = FindObjectsOfType<Ball>();

        if (balls.Length != 0)
        {
            foreach (Ball ball in balls)
            {
                if (ball.GetIsBallFree())
                {
                    Rigidbody2D ballRB = ball.GetComponent<Rigidbody2D>();
                    Ball newBall = Instantiate(ball, ball.transform.position, Quaternion.identity) as Ball;
                    Rigidbody2D newBallRB = newBall.GetComponent<Rigidbody2D>();
                    newBallRB.velocity = ballRB.velocity.normalized * -1 * Time.deltaTime * ball.GetMoveSpeed();
                    newBall.ChooseRandomColor();
                    newBall.SetMoveSpeed(ball.GetMoveSpeed());
                    newBall.SetIsBallFree(true);
                }
            }
        }
    }

    // Verify and creates the shooter
    private void CreateShooter()
    {
        // Finds and cancel case have one already
        Shooter shooter = FindObjectOfType<Shooter>();
        if (shooter) return;

        shooter = Instantiate(shooterPrefab, paddle.transform.position, Quaternion.identity) as Shooter;
        shooter.transform.parent = paddle.transform;
    }

    // Finds and makes all unbreakables blocks to breakable
    private void MakeUnbreakableToBreakable()
    {
        if (GameObject.FindGameObjectsWithTag(NamesTags.GetUnbreakableBlockTag()).Length != 0)
        {
            GameObject[] unbreakables = GameObject.FindGameObjectsWithTag(NamesTags.GetUnbreakableBlockTag());
            foreach (GameObject unbreakable in unbreakables)
            {
                unbreakable.tag = NamesTags.GetBreakableBlockTag();
                gameSession.CountBlocks();
                unbreakable.GetComponent<Animator>().enabled = false;

                for (int i = 0; i < unbreakable.transform.childCount; i++)
                {
                    GameObject child = unbreakable.transform.GetChild(i).gameObject;
                    Destroy(child);
                }
            }
        }
    }
}