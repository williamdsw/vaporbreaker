﻿using Controllers.Panel;
using Core;
using MVC.BL;
using MVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Controllers.Core
{
    /// <summary>
    /// Controller for Game Session
    /// </summary>
    public class GameSessionController : MonoBehaviour
    {
        // || Inspector References

        [Header("Required Elements")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private Block[] blockPrefabs;
        [SerializeField] private PrefabSpawnerController powerUpSpawnerPrefab;

        [Header("UI Texts")]
        [SerializeField] private TextMeshProUGUI powerUpNameLabel;
        [SerializeField] private TextMeshProUGUI ellapsedTimeLabel;
        [SerializeField] private TextMeshProUGUI currentScoreLabel;
        [SerializeField] private TextMeshProUGUI comboMultiplierLabel;
        [SerializeField] private TextMeshProUGUI ballCountdownLabel;

        // || State

        private Enumerators.GameStates actualGameState = Enumerators.GameStates.GAMEPLAY;
        private Vector2Int minMaxNumberOfRandomBlocks = Vector2Int.zero;
        private bool areBallOnFire = false;
        private float ellapsedTime = 0f;
        private int bestCombo = 0;
        private int currentNumberOfBlocks = 0;
        private int currentScore = 0;
        private int numberOfBlocksDestroyed = 0;
        private int totalNumberOfBlocks = 0;
        private int songIndex = 0;
        private int numberOfRandomBlocks = 0;


        // || Config

        private const float TIME_TO_PUT_OUT_FIRE_BALL = 5f;

        // || Cached

        private CanvasGroup canvasGroup;
        private Dictionary<string, Block> blocksPrefabsDictionary;

        // || Properties

        public static GameSessionController Instance { get; private set; }

        public Enumerators.GameStates ActualGameState
        {
            get => actualGameState;
            set
            {
                actualGameState = value;
                switch (ActualGameState)
                {
                    case Enumerators.GameStates.LEVEL_COMPLETE:
                        PausePanelController.Instance.CanPause = false;
                        canvasGroup.interactable = true;
                        break;

                    case Enumerators.GameStates.GAMEPLAY:
                        Time.timeScale = 1f;
                        PausePanelController.Instance.CanPause = true;
                        canvasGroup.interactable = true;
                        break;

                    case Enumerators.GameStates.PAUSE:
                        Time.timeScale = 0f;
                        PausePanelController.Instance.CanPause = true;
                        canvasGroup.interactable = true;
                        break;

                    case Enumerators.GameStates.TRANSITION:
                        PausePanelController.Instance.CanPause = false;
                        canvasGroup.interactable = false;
                        break;

                    default: break;
                }
            }
        }

        public Enumerators.Directions BlockDirection { get; set; } = Enumerators.Directions.None;
        public float StartTimeToSpawnAnotherBall => 10f;
        public float TimeToSpawnAnotherBall { get; set; } = -1f;
        public int CurrentNumberOfBalls { get; set; } = 1;
        public int ComboMultiplier { get; private set; } = 0;
        public int MaxNumberOfBalls => 20;
        public bool CanMoveBlocks { get; set; } = false;
        public bool HasStarted { get; set; } = false;
        public bool CanSpawnAnotherBall { private get; set; } = false;

        private void Awake()
        {
            SetupSingleton();
            FillBlockDictionary();
        }

        private void Start()
        {
            canvasGroup = FindObjectOfType<CanvasGroup>();

            Level level = new LevelBL().GetById(GameStatusController.Instance.LevelId);
            Layout layout = JsonConvert.DeserializeObject<Layout>(level.Layout);
            LoadLevelLayout(layout);

            AudioController.Instance.GetTracks();
            songIndex = UnityEngine.Random.Range(0, AudioController.Instance.AllNotLoopedSongs.Length);
            AudioClip currentClip = AudioController.Instance.AllNotLoopedSongs[songIndex];
            Track current = AudioController.Instance.Tracks.Find(t => t.FileName.Equals(currentClip.name));
            AudioController.Instance.ChangeMusic(currentClip, false, string.Empty, false, true, current);

            powerUpNameLabel.text = string.Empty;

            ResetCombo();
            UpdateUI();

            if (layout.CanChooseRandomBlocks)
            {
                ChooseRandomPowerUpsBlocks();
            }

            if (layout.HasPrefabSpawner)
            {
                SpawnPrefabSpawner();
            }

            FillBlockGrid();
        }

        private void Update()
        {
            FindCamera();

            if (ActualGameState == Enumerators.GameStates.GAMEPLAY)
            {
                if (HasStarted)
                {
                    ShowEllapsedTime();
                    CheckSpawnAnotherBall();
                }
            }
        }

        /// <summary>
        /// Setup singleton instance
        /// </summary>
        private void SetupSingleton()
        {
            if (FindObjectsOfType(GetType()).Length > 1)
            {
                gameObject.SetActive(false);
                DestroyInstance();
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Fill block prefab dictionary
        /// </summary>
        private void FillBlockDictionary()
        {
            try
            {
                blocksPrefabsDictionary = new Dictionary<string, Block>();
                foreach (Block item in blockPrefabs)
                {
                    blocksPrefabsDictionary.Add(item.name, item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Load level's layout
        /// </summary>
        /// <param name="layout"> Instance of Layout </param>
        private void LoadLevelLayout(Layout layout)
        {
            try
            {
                foreach (Layout.BlockInfo info in layout.Blocks)
                {
                    Block clone = Instantiate(blocksPrefabsDictionary[info.PrefabName], new Vector3(info.Position.X, info.Position.Y, info.Position.Z), Quaternion.identity) as Block;
                    clone.transform.SetParent(FindOrCreateObjectParent(NamesTags.Parents.Blocks).transform);
                    clone.SetColor(new Color32(info.Color.R, info.Color.G, info.Color.B, 255));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update UI Elements
        /// </summary>
        public void UpdateUI()
        {
            try
            {
                currentScoreLabel.SetText(Formatter.FormatToCurrency(currentScore));
                ellapsedTimeLabel.text = Formatter.GetEllapsedTimeInHours((int)ellapsedTime);

                if (ComboMultiplier > 1)
                {
                    bestCombo = (ComboMultiplier >= bestCombo ? ComboMultiplier : bestCombo);
                    comboMultiplierLabel.text = string.Concat("x", ComboMultiplier);
                }
                else
                {
                    comboMultiplierLabel.SetText(string.Empty);
                }

                if (CanSpawnAnotherBall)
                {
                    int ballCountdown = (int)(StartTimeToSpawnAnotherBall - TimeToSpawnAnotherBall);
                    ballCountdownLabel.SetText(ballCountdown.ToString("00"));
                }
                else
                {
                    ballCountdownLabel.SetText(string.Empty);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Add value to current score
        /// </summary>
        /// <param name="value"></param>
        public void AddToScore(int value)
        {
            currentScore += value;
            UpdateUI();
        }

        /// <summary>
        /// Count current number of blocks
        /// </summary>
        public void CountBlocks()
        {
            totalNumberOfBlocks++;
            currentNumberOfBlocks = (totalNumberOfBlocks - numberOfBlocksDestroyed);
            UpdateUI();
        }

        /// <summary>
        /// Update number of blocks destroyed
        /// </summary>
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

        /// <summary>
        /// Shows level complete panel
        /// </summary>
        public void CallLevelComplete()
        {
            ActualGameState = Enumerators.GameStates.LEVEL_COMPLETE;
            LevelCompletePanelController.Instance.CallLevelComplete(ellapsedTime, bestCombo, currentScore);
        }

        /// <summary>
        /// Increments combo multiplier
        /// </summary>
        public void AddToComboMultiplier()
        {
            ComboMultiplier++;
            UpdateUI();
        }

        /// <summary>
        /// Reset combo multiplier
        /// </summary>
        public void ResetCombo()
        {
            ComboMultiplier = 0;
            UpdateUI();
        }

        /// <summary>
        /// Choose random blocks to spawn power ups
        /// </summary>
        private void ChooseRandomPowerUpsBlocks()
        {
            try
            {
                GameObject[] blocks = GameObject.FindGameObjectsWithTag(NamesTags.Tags.Breakable);
                if (blocks.Length == 0) return;

                minMaxNumberOfRandomBlocks.x = Mathf.FloorToInt((blocks.Length * (5 / 100f)));
                minMaxNumberOfRandomBlocks.y = Mathf.FloorToInt((blocks.Length * (10f / 100f)));
                numberOfRandomBlocks = UnityEngine.Random.Range(minMaxNumberOfRandomBlocks.x, minMaxNumberOfRandomBlocks.y + 1);

                // Instantiates new power up blocks
                List<GameObject> oldBlocks = new List<GameObject>();
                for (int i = 1; i <= numberOfRandomBlocks; i++)
                {
                    int index = UnityEngine.Random.Range(0, blocks.Length);
                    Block old = blocks[index].GetComponent<Block>();
                    Block powerUp = Instantiate(blocksPrefabsDictionary[NamesTags.Blocks.PowerUpBlock], old.transform.position, Quaternion.identity) as Block;
                    powerUp.transform.SetParent(FindOrCreateObjectParent(NamesTags.Parents.Blocks).transform);
                    powerUp.SetColor(old.ParticlesColor);
                    oldBlocks.Add(old.gameObject);
                }

                // Remove old standard blocks
                foreach (GameObject block in oldBlocks)
                {
                    Destroy(block);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Spawn prefab spawner
        /// </summary>
        private void SpawnPrefabSpawner()
        {
            try
            {
                Instantiate(powerUpSpawnerPrefab, Vector3.zero, Quaternion.identity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Move blocks at direction
        /// </summary>
        /// <param name="direction"> Desired direction </param>
        public void MoveBlocks(Enumerators.Directions direction)
        {
            try
            {
                if (ActualGameState == Enumerators.GameStates.GAMEPLAY)
                {
                    if (CanMoveBlocks)
                    {
                        Block[] blocks = FindObjectsOfType<Block>();
                        int numberOfOcorrences = 0;
                        foreach (Block block in blocks)
                        {
                            if (block.CompareTag(NamesTags.Tags.Unbreakable)) return;

                            switch (direction)
                            {
                                case Enumerators.Directions.Right:
                                    Vector3 right = new Vector3(block.transform.position.x + 1f, block.transform.position.y, 0f);
                                    MoveBlockAtPosition(block, right, (right.x <= BlockGrid.MaxCoordinatesInXY.x), ref numberOfOcorrences);
                                    break;

                                case Enumerators.Directions.Left:
                                    Vector3 left = new Vector3(block.transform.position.x - 1f, block.transform.position.y, 0f);
                                    MoveBlockAtPosition(block, left, (left.x >= BlockGrid.MinCoordinatesInXY.x), ref numberOfOcorrences);
                                    break;

                                case Enumerators.Directions.Down:
                                    Vector3 down = new Vector3(block.transform.position.x, block.transform.position.y - 0.5f, 0f);
                                    MoveBlockAtPosition(block, down, down.y >= BlockGrid.MinCoordinatesInXY.y, ref numberOfOcorrences);
                                    break;

                                case Enumerators.Directions.Up:
                                    Vector3 up = new Vector3(block.transform.position.x, block.transform.position.y + 0.5f, 0f);
                                    MoveBlockAtPosition(block, up, up.y <= BlockGrid.MaxCoordinatesInXY.y, ref numberOfOcorrences);
                                    break;

                                default: break;
                            }
                        }

                        CanMoveBlocks = (numberOfOcorrences >= 1);
                        if (CanMoveBlocks)
                        {
                            AudioController.Instance.PlaySFX(AudioController.Instance.HittingWallSound, AudioController.Instance.MaxSFXVolume);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Move block at position
        /// </summary>
        /// <param name="block"> Block instance </param>
        /// <param name="position"> Desired position </param>
        /// <param name="expression"> Expression to be evaluated </param>
        /// <param name="numberOfOcorrences"> Number of blocks moved </param>
        private void MoveBlockAtPosition(Block block, Vector3 position, bool expression, ref int numberOfOcorrences)
        {
            if (expression && BlockGrid.CheckPosition(position) && BlockGrid.GetBlock(position) == null)
            {
                BlockGrid.RedefineBlock(block.transform.position, null);
                BlockGrid.RedefineBlock(position, block);
                block.transform.position = position;
                numberOfOcorrences++;
            }
        }

        /// <summary>
        /// Fills block grid
        /// </summary>
        private void FillBlockGrid()
        {
            try
            {
                BlockGrid.InitGrid();

                foreach (Block block in FindObjectsOfType<Block>())
                {
                    if (BlockGrid.CheckPosition(block.transform.position) && BlockGrid.GetBlock(block.transform.position) == null)
                    {
                        BlockGrid.RedefineBlock(block.transform.position, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Make all balls to fire balls
        /// </summary>
        public void MakeFireBalls()
        {
            try
            {
                if (ActualGameState == Enumerators.GameStates.GAMEPLAY)
                {
                    if (areBallOnFire)
                    {
                        CancelInvoke(NamesTags.Functions.UndoFireBalls);
                        Invoke(NamesTags.Functions.UndoFireBalls, TIME_TO_PUT_OUT_FIRE_BALL);
                    }
                    else
                    {
                        areBallOnFire = true;

                        foreach (GameObject blockObject in GameObject.FindGameObjectsWithTag(NamesTags.Tags.Breakable))
                        {
                            Block block = blockObject.GetComponent<Block>();
                            block.MaxHits = 1;
                            block.BoxCollider2D.isTrigger = true;
                        }

                        foreach (Ball ball in FindObjectsOfType<Ball>())
                        {
                            ball.IsBallOnFire = true;
                            ball.ChangeBallSprite(true);
                        }

                        AudioController.Instance.PlayME(AudioController.Instance.FireEffect, AudioController.Instance.MaxMEVolume / 2f, true);

                        Invoke(NamesTags.Functions.UndoFireBalls, TIME_TO_PUT_OUT_FIRE_BALL);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Undo fire balls effect
        /// </summary>
        private void UndoFireBalls()
        {
            try
            {
                areBallOnFire = false;

                foreach (GameObject blockObject in GameObject.FindGameObjectsWithTag(NamesTags.Tags.Breakable))
                {
                    Block block = blockObject.GetComponent<Block>();
                    block.MaxHits = block.StartMaxHits;
                    block.BoxCollider2D.isTrigger = false;
                }

                foreach (Ball ball in FindObjectsOfType<Ball>())
                {
                    ball.IsBallOnFire = false;
                    ball.ChangeBallSprite(false);
                }

                AudioController.Instance.StopME();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Find or create object parent
        /// </summary>
        /// <param name="parentName"> Desired name </param>
        /// <returns> Instance of Object </returns>
        public GameObject FindOrCreateObjectParent(string parentName)
        {
            GameObject parent = GameObject.Find(parentName);
            if (!parent)
            {
                parent = new GameObject(parentName);
            }

            return parent;
        }

        /// <summary>
        /// Find main camera
        /// </summary>
        private void FindCamera()
        {
            if (canvas.worldCamera) return;
            canvas.worldCamera = Camera.main;
        }

        /// <summary>
        /// Show current ellapsed time
        /// </summary>
        private void ShowEllapsedTime()
        {
            ellapsedTime += Time.deltaTime;
            UpdateUI();
        }

        /// <summary>
        /// Check if can spawn another ball
        /// </summary>
        private void CheckSpawnAnotherBall()
        {
            if (CanSpawnAnotherBall)
            {
                if (CurrentNumberOfBalls >= MaxNumberOfBalls)
                {
                    TimeToSpawnAnotherBall = -1f;
                    return;
                }

                TimeToSpawnAnotherBall += Time.deltaTime;

                if (TimeToSpawnAnotherBall >= StartTimeToSpawnAnotherBall)
                {
                    Ball[] balls = FindObjectsOfType<Ball>();
                    if (balls.Length != 0)
                    {
                        foreach (Ball ball in balls)
                        {
                            if (CurrentNumberOfBalls >= MaxNumberOfBalls) break;

                            Ball newBall = Instantiate(ball, ball.transform.position, Quaternion.identity) as Ball;
                            newBall.Velocity = (ball.Velocity.normalized * -1 * Time.fixedDeltaTime * ball.MoveSpeed);
                            newBall.MoveSpeed = ball.MoveSpeed;
                            if (ball.IsBallOnFire)
                            {
                                newBall.IsBallOnFire = true;
                                newBall.ChangeBallSprite(newBall.IsBallOnFire);
                            }

                            CurrentNumberOfBalls++;
                        }
                    }

                    TimeToSpawnAnotherBall = -1f;
                }
            }
        }

        /// <summary>
        /// Reset current objects
        /// </summary>
        private void ResetObjects()
        {
            try
            {
                foreach (Ball ball in FindObjectsOfType<Ball>())
                {
                    Destroy(ball.gameObject);
                }

                foreach (Shooter shooter in FindObjectsOfType<Shooter>())
                {
                    Destroy(shooter.gameObject);
                }

                foreach (PowerUp powerUp in FindObjectsOfType<PowerUp>())
                {
                    Destroy(powerUp.gameObject);
                }

                Destroy(FindObjectOfType<Paddle>().gameObject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Reset current level if last ball touches the Death Zone
        /// </summary>
        public void ResetLevel()
        {
            try
            {
                TimeToSpawnAnotherBall = -1f;
                CurrentNumberOfBalls = 0;
                currentScore = 0;
                bestCombo = 0;
                CanSpawnAnotherBall = false;
                HasStarted = false;
                CanMoveBlocks = false;
                BlockDirection = Enumerators.Directions.None;
                ResetCombo();
                UpdateUI();

                UndoFireBalls();
                ResetObjects();
                SceneManagerController.ReloadScene();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Reset current level
        /// </summary>
        /// <param name="sceneName"> Next scene name </param>
        public void GotoScene(string sceneName)
        {
            AudioController.Instance.StopMusic();
            GameStatusController.Instance.NextSceneName = sceneName;
            GameStatusController.Instance.HasStartedSong = false;
            GameStatusController.Instance.CameFromLevel = true;
            SceneManagerController.CallScene(SceneManagerController.SceneNames.Loading);
            DestroyInstance();
        }

        /// <summary>
        /// Destroy this GameObject
        /// </summary>
        public void DestroyInstance() => Destroy(gameObject);

        /// <summary>
        /// Shows power up name
        /// </summary>
        /// <param name="name"> Power up translated name </param>
        public void ShowPowerUpName(string name)
        {
            StopCoroutine(HidePowerUpName());
            powerUpNameLabel.text = name;
            StartCoroutine(HidePowerUpName());
        }

        /// <summary>
        /// Hides power up name
        /// </summary>
        private IEnumerator HidePowerUpName()
        {
            yield return new WaitForSeconds(3f);
            powerUpNameLabel.text = string.Empty;
        }

        /// <summary>
        /// Change current track to next
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void OnChangeTrack(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed && callbackContext.ReadValueAsButton())
            {
                if (ActualGameState == Enumerators.GameStates.GAMEPLAY || ActualGameState == Enumerators.GameStates.PAUSE)
                {
                    songIndex++;
                    songIndex = (songIndex >= AudioController.Instance.AllNotLoopedSongs.Length ? 0 : songIndex);
                    AudioController.Instance.StopAllCoroutines();
                    AudioClip nextClip = AudioController.Instance.AllNotLoopedSongs[songIndex];
                    Track nextTrack = AudioController.Instance.Tracks.Find(t => t.FileName.Equals(nextClip.name));
                    AudioController.Instance.ChangeMusic(nextClip, false, string.Empty, false, true, nextTrack);
                }
            }
        }

        /// <summary>
        /// Release first ball or shoot
        /// </summary>
        /// <param name="callbackContext"> Context with parameters </param>
        public void OnReleaseBallOrShoot(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed && callbackContext.ReadValueAsButton())
            {
                if (ActualGameState == Enumerators.GameStates.GAMEPLAY)
                {
                    if (!HasStarted)
                    {
                        Ball ball = FindObjectOfType<Ball>();
                        if (ball != null)
                        {
                            ball.LaunchBall();
                        }
                    }
                    else
                    {
                        Shooter shooter = FindObjectOfType<Shooter>();
                        if (shooter != null)
                        {
                            shooter.Shoot();
                        }
                    }
                }
            }
        }
    }
}