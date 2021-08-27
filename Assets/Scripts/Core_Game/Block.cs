using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Utilities;

public enum BlockColors
{
    Artic, Blue, Brown, Butter, Cerulean, Dark_Green, Green, Lilac, Ocean, Olive,
    Orange, Periwinkle, Pine, Pink, Purple, Red, Ruby, Silver, Strawberry, Teal, Yellow
}

public class Block : MonoBehaviour
{
    // Configuration
    private float minPointsScore = 1f;
    private float maxPointsScore = 1000f;
    [SerializeField] private bool hasRandomHits = false;
    [SerializeField] private Sprite[] hitSprites;
    [SerializeField] private GameObject[] explosionPrefabs;
    [SerializeField] private GameObject particlesPrefab;
    [SerializeField] private FlashTextEffect blockScoreText;
    [SerializeField] private PowerUp[] powerUpPrefabs;
    [SerializeField] private Block powerUpBlockPrefab;
    [SerializeField] private BlockColors blockColor;
    private Color32 particlesColor;

    // State variables
    private int maxHits = 0;
    private int timesHit = 0;
    private bool collidedWithBall = false;
    private bool isBallBig = false;
    private bool lastCollision = false;
    [SerializeField] private bool canSpawnPowerUpBlock = false;
    public Dictionary<string, int> listPowerUpIndexes = new Dictionary<string, int>();

    // Cached Components
    private SpriteRenderer spriteRenderer;

    // Cached Objects
    private AudioController audioController;
    private GameSession gameSession;

    public void SetCanSpawnPowerUpBlock(bool canSpawnPowerUpBlock)
    {
        this.canSpawnPowerUpBlock = canSpawnPowerUpBlock;
    }

    public void SetMaxHits(int maxHits)
    {
        this.maxHits = maxHits;
    }

    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Find others
        gameSession = FindObjectOfType<GameSession>();
        audioController = FindObjectOfType<AudioController>();

        CountBreakableBlocks();

        maxHits = (hasRandomHits ? (int)Random.Range(1, hitSprites.Length + 2) : hitSprites.Length + 1);

        // Lock power up blocker to spawn another power up blocker
        if (powerUpPrefabs.Length != 0)
        {
            canSpawnPowerUpBlock = false;

            // Fill list
            int index = 0;
            foreach (PowerUp powerUp in powerUpPrefabs)
            {
                listPowerUpIndexes.Add(powerUp.name, index);
                index++;
            }
        }

        DefineParticlesColor(blockColor);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (gameSession.GetActualGameState() == GameState.GAMEPLAY)
        {
            if (!lastCollision)
            {
                // Verifies the ball
                collidedWithBall = (other.gameObject.GetComponent<Ball>() != null);

                if (CompareTag(NamesTags.BreakableBlockTag))
                {
                    HandleHit();
                }
                else if (CompareTag(NamesTags.UnbreakableBlockTag))
                {
                    audioController.PlaySFX(audioController.MetalPingSound, audioController.GetMaxSFXVolume() / 2);
                }

                if (collidedWithBall)
                {
                    GameObject ball = other.gameObject;
                    isBallBig = ball.transform.localScale.y >= 8f;
                    if (ball.transform.localScale.y >= 8f)
                    {
                        CameraImpulse.Instance.TriggerImpulse();
                    }
                }
            }
        }
    }

    private void CountBreakableBlocks()
    {
        if (CompareTag(NamesTags.BreakableBlockTag))
        {
            gameSession.CountBlocks();
        }
    }

    // Handles the ball hit
    private void HandleHit()
    {
        timesHit++;

        if (timesHit >= maxHits)
        {
            DestroyBlock();
            lastCollision = true;
        }
        else
        {
            ShowNextSprite();
        }
    }

    // Shows next sprite if have
    private void ShowNextSprite()
    {
        int spriteIndex = timesHit - 1;
        if (hitSprites[spriteIndex] != null)
        {
            spriteRenderer.sprite = hitSprites[spriteIndex];
        }

        // SFX & VFX
        AudioClip clip = (isBallBig ? audioController.HittingFace : audioController.SlamSound);
        audioController.PlaySFX(clip, audioController.GetMaxSFXVolume() / 2f);
        SpawnDebris(50f, 100f);
        print(clip.name);
    }

    // Do a lot of SFX and VFX stuff, update GameSession and destroys itself
    private void DestroyBlock()
    {
        // Add to combo only it's the ball
        if (collidedWithBall)
        {
            gameSession.AddToComboMultiplier();
        }

        // Calculates score
        int comboMultiplier = gameSession.GetComboMultiplier();
        int score = (int)Random.Range(minPointsScore, maxPointsScore);
        score *= maxHits;

        // Multiply score only it's the ball
        if (collidedWithBall)
        {
            if (comboMultiplier > 1)
            {
                score *= comboMultiplier;
            }
        }

        // VFX
        TriggerExplosion();
        SpawnDebris(100f, 200f);

        // Case not the power up block
        if (powerUpPrefabs.Length == 0)
        {
            ShowScoreText(score);
            gameSession.AddToStore(score);
        }
        else
        {
            SpawnPowerUp();
        }

        // Only "normal" blocks can spawn
        if (canSpawnPowerUpBlock && powerUpPrefabs.Length == 0)
        {
            SpawnPowerUpBlock();
        }

        StartCoroutine(DestroyCoroutine());
    }

    // Destroys object and update Game Session
    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
        gameSession.BlockDestroyed();
    }

    private void DefineParticlesColor(BlockColors blockColor)
    {
        switch (blockColor)
        {
            case BlockColors.Artic: { particlesColor = new Color32(23, 255, 255, 255); break; }
            case BlockColors.Blue: { particlesColor = Color.blue; break; }
            case BlockColors.Brown: { particlesColor = new Color32(144, 88, 64, 255); break; }
            case BlockColors.Butter: { particlesColor = new Color32(222, 148, 99, 255); break; }
            case BlockColors.Cerulean: { particlesColor = new Color32(66, 165, 255, 255); break; }
            case BlockColors.Dark_Green: { particlesColor = new Color32(0, 64, 32, 255); break; }
            case BlockColors.Green: { particlesColor = Color.green; break; }
            case BlockColors.Lilac: { particlesColor = new Color32(205, 128, 209, 255); break; }
            case BlockColors.Ocean: { particlesColor = new Color32(0, 214, 181, 255); break; }
            case BlockColors.Olive: { particlesColor = new Color32(139, 173, 15, 255); break; }
            case BlockColors.Orange: { particlesColor = new Color32(255, 107, 0, 255); break; }
            case BlockColors.Periwinkle: { particlesColor = new Color32(177, 108, 248, 255); break; }
            case BlockColors.Pine: { particlesColor = new Color32(0, 96, 96, 255); break; }
            case BlockColors.Pink: { particlesColor = new Color32(208, 47, 216, 255); break; }
            case BlockColors.Purple: { particlesColor = new Color32(74, 16, 107, 255); break; }
            case BlockColors.Red: { particlesColor = Color.red; break; }
            case BlockColors.Ruby: { particlesColor = new Color32(64, 0, 0, 255); break; }
            case BlockColors.Silver: { particlesColor = Color.gray; break; }
            case BlockColors.Strawberry: { particlesColor = new Color32(198, 49, 82, 255); break; }
            case BlockColors.Teal: { particlesColor = new Color32(91, 255, 255, 255); break; }
            case BlockColors.Yellow: { particlesColor = Color.yellow; break; }
            default: { particlesColor = Color.white; break; }
        }
    }

    // Shows a random animated explosion
    private void TriggerExplosion()
    {
        if (explosionPrefabs.Length >= 1)
        {
            audioController.PlaySFX(audioController.ExplosionSound, audioController.GetMaxSFXVolume() / 2);
            int randomIndex = Random.Range(0, explosionPrefabs.Length);
            GameObject explosion = Instantiate(explosionPrefabs[randomIndex], this.transform.position, Quaternion.identity) as GameObject;
            explosion.transform.SetParent(gameSession.FindOrCreateObjectParent(NamesTags.ExplosionsParentName).transform);
            Animator animator = explosion.GetComponent<Animator>();
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(explosion, animationLength);
        }
    }

    // Shows the text with score
    private void ShowScoreText(int score)
    {
        if (blockScoreText)
        {
            TextMeshPro textMeshPro = blockScoreText.GetComponentInChildren<TextMeshPro>();
            textMeshPro.text = score.ToString();
            GameObject scoreText = Instantiate(blockScoreText.gameObject, transform.position, Quaternion.identity) as GameObject;
            scoreText.transform.SetParent(gameSession.FindOrCreateObjectParent(NamesTags.BlockScoreTextParentName).transform);
            Animator animator = scoreText.GetComponent<Animator>();
            float durationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(scoreText, durationLength);
        }
    }

    // Shows random number of debris
    private void SpawnDebris(float minParticles, float maxParticles)
    {
        if (particlesPrefab)
        {
            // Instantiate and Destroy
            GameObject debris = Instantiate(particlesPrefab, this.transform.position, particlesPrefab.transform.rotation) as GameObject;
            debris.transform.SetParent(gameSession.FindOrCreateObjectParent(NamesTags.DebrisParentName).transform);

            // Color
            ParticleSystem debrisParticleSystem = debris.GetComponent<ParticleSystem>();
            var mainModule = debrisParticleSystem.main;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(particlesColor);

            // Time to destroy
            ParticleSystem prefabParticleSystem = particlesPrefab.GetComponent<ParticleSystem>();
            float durationLength = prefabParticleSystem.main.duration + prefabParticleSystem.main.startLifetime.constant;
            Destroy(debris, durationLength);
        }
    }

    // Instantiate a Power Up Block
    private void SpawnPowerUpBlock()
    {
        if (powerUpBlockPrefab)
        {
            Block powerUpBlock = Instantiate(powerUpBlockPrefab, this.transform.position, this.transform.rotation) as Block;
            powerUpBlock.SetCanSpawnPowerUpBlock(false);
            powerUpBlock.transform.SetParent(gameSession.FindOrCreateObjectParent(NamesTags.BlocksParentName).transform);
        }
    }

    // Instantiate a Power Up
    private void SpawnPowerUp()
    {
        int randomIndex = CalculateIndexChance();
        GameObject powerUp = Instantiate(powerUpPrefabs[randomIndex].gameObject, this.transform.position, Quaternion.identity) as GameObject;
        powerUp.transform.SetParent(gameSession.FindOrCreateObjectParent(NamesTags.PowerUpsParentName).transform);
        audioController.PlaySFX(audioController.ShowUpSound, audioController.GetMaxSFXVolume());
    }

    // Calculate chance percent and possibly decide when have other options... 
    private int CalculateIndexChance()
    {
        int index = 0;
        int chance = Random.Range(0, 100);
        if (chance >= 99)
        {
            index = listPowerUpIndexes[NamesTags.PowerUpLevelCompleteName];
        }
        else if (chance >= 80 && chance < 95)
        {
            string[] powerUps = { NamesTags.PowerUpShooterName, NamesTags.PowerUpZeroDeathsName };
            index = listPowerUpIndexes[powerUps[Random.Range(0, powerUps.Length)]];
        }
        else if (chance >= 60 && chance < 80)
        {
            index = listPowerUpIndexes[NamesTags.PowerUpAllBlocksOneHitName];
        }
        else if (chance >= 50 && chance < 60)
        {
            index = listPowerUpIndexes[NamesTags.PowerUpUnbreakablesToBreakablesName];
        }
        else if (chance >= 0 && chance < 50)
        {
            string[] powerUps = 
            {
                NamesTags.PowerUpBallBiggerName, NamesTags.PowerUpBallFasterName,
                NamesTags.PowerUpBallSlowerName, NamesTags.PowerUpBallSmallerName,
                NamesTags.PowerUpDuplicateBallName, NamesTags.PowerUpPaddleExpandName,
                NamesTags.PowerUpPaddleShrinkName, NamesTags.PowerUpResetBallName,
                NamesTags.PowerUpResetPaddleName, NamesTags.PowerUpRandomName
            };
            index = listPowerUpIndexes[powerUps[Random.Range(0, powerUps.Length)]];
        }

        return index;
    }
}