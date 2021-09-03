using Controllers.Core;
using Luminosity.IO;
using MVC.Global;
using TMPro;
using UnityEngine;
using Utilities;

public enum GameState
{
    LEVEL_COMPLETE, GAMEPLAY, PAUSE, SAVE_LOAD, TRANSITION
}

public class GameSession : MonoBehaviour
{
    [Header("Configuration Parameters")]
    [Range(0f, 10f)] [SerializeField] private float gameSpeed = 1f;
    [SerializeField] private bool isAutoplayEnabled;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI numberOfDeathsText;
    [SerializeField] private TextMeshProUGUI powerUpNameText;
    [SerializeField] private TextMeshProUGUI ellapsedTimeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboMultiplierText;

    [SerializeField] private Canvas canvas;

    // State
    private int bestCombo = 0;
    private int comboMultiplier = 0;
    private int currentNumberOfBlocks = 0;
    private int currentNumberOfLaunchedBalls = 0;
    private int currentScore = 0;
    private int numberOfBlocksDestroyed = 0;
    private int numberOfDeaths = 0;
    private int totalNumberOfBlocks = 0;
    private float ellapsedTime = 0f;
    private bool hasStarted = false;

    // CONS
    private const int MAX_NUMBER_OF_LAUNCHED_BALLS = 5;

    // Random blocks to spawn another power up block
    [SerializeField] private bool chooseRandomBlocks = false;
    private int minNumberOfRandomBlocks = 0;
    private int maxNumberOfRandomBlocks = 0;
    private int numberOfRandomBlocks = 0;
    private GameState actualGameState = GameState.GAMEPLAY;

    // Power Up informations
    private bool hasPowerUpCollidedWithPaddle = false;
    private string powerUpName = "";
    private float startTimeToHidePowerUp = 3f;
    private float timeToHidePowerUp = 0;

    [SerializeField] private GameObject ballPrefab;
    private bool canSpawnAnotherBall = false;
    private float startTimeToSpawnAnotherBall = 0f;
    private float timeToSpawnAnotherBall = 0;

    // Change music in game
    private bool canCalculate = false;
    private bool canChangeMusicInGame = true;
    private float startTimeToUnlockButton = 10f;
    private float timeToUnlockButton = 10f;
    private int songIndex = 0;

    // Cached 
    private AudioController audioController;
    private Ball[] balls;
    private CanvasGroup canvasGroup;
    private CursorController cursorController;
    private GameStatusController gameStatusController;
    private LevelCompleteController levelCompleteController;
    private Paddle paddle;
    private Pause pauseController;

    public GameState GetActualGameState()
    {
        return actualGameState;
    }
    public int GetComboMultiplier()
    {
        return comboMultiplier;
    }

    public bool GetIsAutoplayEnabled()
    {
        return isAutoplayEnabled;
    }

    public bool GetHasPowerUpCollidedWithPaddle()
    {
        return hasPowerUpCollidedWithPaddle;
    }

    public bool GetHasStarted()
    {
        return hasStarted;
    }

    public string GetPowerUpName()
    {
        return powerUpName;
    }

    public void SetCanSpawnAnotherBall(bool canSpawnAnotherBall)
    {
        this.canSpawnAnotherBall = canSpawnAnotherBall;
    }

    public void SetHasPowerUpCollidedWithPaddle(bool hasPowerUpCollidedWithPaddle)
    {
        this.hasPowerUpCollidedWithPaddle = hasPowerUpCollidedWithPaddle;
    }

    public void SetHasStarted(bool hasStarted)
    {
        this.hasStarted = hasStarted;
    }

    public void SetPowerUpName(string powerUpName)
    {
        this.powerUpName = powerUpName;
    }

    public void SetStartTimeToSpawnAnotherBall(float startTimeToSpawnAnotherBall)
    {
        this.startTimeToSpawnAnotherBall = startTimeToSpawnAnotherBall;
    }

    public void SetActualGameState(GameState gameState)
    {
        this.actualGameState = gameState;

        switch (actualGameState)
        {
            case GameState.LEVEL_COMPLETE:
            case GameState.TRANSITION:
                {
                    pauseController.SetCanPause(false);
                    canvasGroup.interactable = false;
                    break;
                }

            case GameState.GAMEPLAY:
                {
                    Time.timeScale = 1f;
                    pauseController.SetCanPause(true);
                    canvasGroup.interactable = true;
                    break;
                }

            case GameState.PAUSE:
                {
                    Time.timeScale = 0f;
                    pauseController.SetCanPause(true);
                    canvasGroup.interactable = true;
                    break;
                }
        }
    }

    public int CurrentNumberOfLaunchedBalls
    {
        get => this.currentNumberOfLaunchedBalls;
        set => currentNumberOfLaunchedBalls = value;
    }

    private void Awake()
    {
        SetupSingleton();
    }

    private void Start()
    {
        // Find other objects
        audioController = FindObjectOfType<AudioController>();
        balls = FindObjectsOfType<Ball>();
        canvasGroup = FindObjectOfType<CanvasGroup>();
        cursorController = FindObjectOfType<CursorController>();
        gameStatusController = FindObjectOfType<GameStatusController>();
        levelCompleteController = FindObjectOfType<LevelCompleteController>();
        pauseController = FindObjectOfType<Pause>();

        // Balls colors
        foreach (Ball ball in balls)
        {
            ball.ChooseRandomColor();
        }

        // Audio Controller
        songIndex = Random.Range(0, audioController.AllNotLoopedSongs.Length);
        audioController.ChangeMusic(audioController.AllNotLoopedSongs[songIndex], false, "", false, true);

        // Default
        canChangeMusicInGame = false;
        canCalculate = true;

        ResetCombo();
        UpdateUI();

        if (chooseRandomBlocks)
        {
            ChooseBlocks();
        }
    }

    private void Update()
    {
        FindCamera();

        if (actualGameState == GameState.GAMEPLAY)
        {
            Time.timeScale = gameSpeed;

            if (hasStarted)
            {
                ShowEllapsedTime();

                // Hides text after 3 seconds
                if (hasPowerUpCollidedWithPaddle)
                {
                    timeToHidePowerUp += Time.deltaTime;
                    if (timeToHidePowerUp >= startTimeToHidePowerUp)
                    {
                        timeToHidePowerUp = 0;
                        hasPowerUpCollidedWithPaddle = false;
                    }
                }

                if (currentNumberOfLaunchedBalls <= MAX_NUMBER_OF_LAUNCHED_BALLS)
                {
                    if (canSpawnAnotherBall)
                    {
                        timeToSpawnAnotherBall += Time.deltaTime;
                        if (timeToSpawnAnotherBall >= startTimeToSpawnAnotherBall)
                        {
                            if (ballPrefab)
                            {
                                if (!paddle)
                                {
                                    paddle = FindObjectOfType<Paddle>();
                                }

                                Instantiate(ballPrefab, paddle.transform.position, Quaternion.identity);
                            }

                            timeToSpawnAnotherBall = 0;
                            canSpawnAnotherBall = false;
                        }
                    }
                }
            }
        }

        if (actualGameState == GameState.GAMEPLAY || actualGameState == GameState.PAUSE)
        {
            ChangeSong();
        }
    }

    //--------------------------------------------------------------------------------//

    // Setups a singleton
    private void SetupSingleton()
    {
        int numberOfInstances = FindObjectsOfType(GetType()).Length;
        if (numberOfInstances > 1)
        {
            gameObject.SetActive(false);
            DestroyImmediate(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    //--------------------------------------------------------------------------------//

    // Updates Texts in UI
    public void UpdateUI()
    {
        numberOfDeathsText.text = string.Concat("x", numberOfDeaths.ToString());
        scoreText.text = currentScore.ToString();
        ellapsedTimeText.text = Formatter.FormatEllapsedTime((int)ellapsedTime);

        // Combo text
        if (comboMultiplier > 1)
        {
            bestCombo = (comboMultiplier >= bestCombo ? comboMultiplier : bestCombo);
            comboMultiplierText.text = string.Concat("Combo: ", "x", comboMultiplier);
        }
        else
        {
            comboMultiplierText.text = string.Empty;
        }

        // Power Up Name
        powerUpNameText.text = (hasPowerUpCollidedWithPaddle ? powerUpName : string.Empty);
    }

    // Add points to score
    public void AddToStore(int value)
    {
        currentScore += value;
        UpdateUI();
    }

    // Decrement the death count
    public void ZeroNumberOfDeaths()
    {
        numberOfDeaths = 0;
        UpdateUI();
    }

    // Count blocks
    public void CountBlocks()
    {
        totalNumberOfBlocks++;
        currentNumberOfBlocks = (totalNumberOfBlocks - numberOfBlocksDestroyed);
        UpdateUI();
    }

    // Updates number of blocks
    public void BlockDestroyed()
    {
        numberOfBlocksDestroyed++;
        currentNumberOfBlocks = (totalNumberOfBlocks - numberOfBlocksDestroyed);
        UpdateUI();

        if (currentNumberOfBlocks <= 0 && numberOfBlocksDestroyed == totalNumberOfBlocks)
        {
            CallLevelComplete();
        }
    }

    public void CallLevelComplete()
    {
        SetActualGameState(GameState.LEVEL_COMPLETE);
        levelCompleteController.CallLevelComplete(ellapsedTime, numberOfBlocksDestroyed,
                                                   totalNumberOfBlocks, bestCombo, currentScore,
                                                   numberOfDeaths);
    }

    // Increment combo
    public void AddToComboMultiplier()
    {
        comboMultiplier++;
        UpdateUI();
    }

    // Resets combo
    public void ResetCombo()
    {
        comboMultiplier = 0;
        UpdateUI();
    }

    public void ResetHideTime()
    {
        this.timeToHidePowerUp = 0f;
    }

    // Choose random blocks to instantiate Power Up Blocks
    private void ChooseBlocks()
    {
        // Finds
        GameObject[] blocks = GameObject.FindGameObjectsWithTag(NamesTags.BreakableBlockTag);
        if (blocks.Length == 0) return;

        // Calculates
        minNumberOfRandomBlocks = Mathf.FloorToInt((blocks.Length * (10f / 100f)));
        maxNumberOfRandomBlocks = Mathf.FloorToInt((blocks.Length * (20f / 100f)));
        numberOfRandomBlocks = Random.Range(minNumberOfRandomBlocks, maxNumberOfRandomBlocks + 1);

        for (int i = 1; i <= numberOfRandomBlocks; i++)
        {
            int index = Random.Range(0, blocks.Length);
            blocks[index].GetComponent<Block>().SetCanSpawnPowerUpBlock(true);
        }
    }

    //--------------------------------------------------------------------------------//
    // FINDER / CREATER

    // Finds a GameObject by name or create one
    public GameObject FindOrCreateObjectParent(string parentName)
    {
        GameObject parent = GameObject.Find(parentName);
        if (!parent)
        {
            parent = new GameObject(parentName);
        }

        return parent;
    }

    private void FindCamera()
    {
        if (canvas.worldCamera) return;
        canvas.worldCamera = Camera.main;
    }

    //--------------------------------------------------------------------------------//
    // AUDIO RELATED

    private void ChangeSong()
    {
        if (canChangeMusicInGame)
        {
            if (InputManager.GetButtonDown(Configuration.InputsNames.ChangeSong))
            {
                canChangeMusicInGame = false;
                canCalculate = true;
                songIndex++;
                songIndex = (songIndex >= audioController.AllNotLoopedSongs.Length ? 0 : songIndex);
                audioController.StopAllCoroutines();
                audioController.ChangeMusic(audioController.AllNotLoopedSongs[songIndex], false, "", false, true);
                return;
            }
        }

        if (canCalculate)
        {
            timeToUnlockButton -= Time.fixedDeltaTime;
            if (timeToUnlockButton <= 0)
            {
                timeToUnlockButton = startTimeToUnlockButton;
                canCalculate = false;
                canChangeMusicInGame = true;
            }
        }
    }

    //--------------------------------------------------------------------------------//
    // TIME RELATED

    // Increments ellapsed time
    private void ShowEllapsedTime()
    {
        ellapsedTime += Time.deltaTime;
        UpdateUI();
    }

    //--------------------------------------------------------------------------------//
    // RESET RELATED

    private void ResetObjects()
    {
        // Balls
        Ball[] balls = FindObjectsOfType<Ball>();
        foreach (Ball ball in balls)
        {
            ball.transform.localScale = Vector2.one;
            float defaultSpeed = ball.GetDefaultSpeed();
            ball.SetMoveSpeed(defaultSpeed);
        }

        // Shooters
        Shooter[] shooters = FindObjectsOfType<Shooter>();
        if (shooters.Length != 0)
        {
            foreach (Shooter shooter in shooters)
            {
                Destroy(shooter.gameObject);
            }
        }

        // PowerUps
        PowerUp[] powerUps = FindObjectsOfType<PowerUp>();
        if (powerUps.Length != 0)
        {
            foreach (PowerUp powerUp in powerUps)
            {
                Destroy(powerUp.gameObject);
            }
        }

        // Paddle
        Paddle paddle = FindObjectOfType<Paddle>();
        if (paddle)
        {
            paddle.ResetPaddle();
        }
    }

    // Resets level if ball touches 'death zone'
    public void ResetLevel()
    {
        numberOfDeaths++;
        timeToSpawnAnotherBall = 0;
        currentNumberOfLaunchedBalls = 0;
        currentScore = 0;
        canSpawnAnotherBall = false;
        ResetCombo();
        UpdateUI();
        hasStarted = false;
        hasPowerUpCollidedWithPaddle = false;

        // Reset objects
        ResetObjects();
        SceneManagerController.ReloadScene();
    }

    // Destroys this
    public void ResetGame(string sceneName)
    {
        audioController.StopMusic();
        gameStatusController.SetNextSceneName(sceneName);
        gameStatusController.SetHasStartedSong(false);
        SceneManagerController.CallScene(SceneManagerController.GetLoadingSceneName());
        Destroy(this.gameObject);
    }
}