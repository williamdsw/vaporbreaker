using Controllers.Core;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Utilities;

namespace Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Block : MonoBehaviour
    {
        // || Inspector References

        [Header("Required Configuration Parameters")]
        [SerializeField] private Sprite[] hitSprites;
        [SerializeField] private GameObject[] explosionPrefabs;
        [SerializeField] private GameObject particlesPrefab;
        [SerializeField] private GameObject blockScoreTextPrefab;
        [SerializeField] private PowerUp[] powerUpPrefabs;
        [SerializeField] private bool hasRandomHits = false;
        [SerializeField] private bool spawnPowerUp = false;

        // || Const

        private readonly Vector2 minMaxPointsScore = new Vector2(1f, 1000f);

        // || State

        private int timesHit = 0;
        private bool collidedWithBall = false;
        private bool isBallBig = false;
        private bool lastCollision = false;

        // || Cached

        private SpriteRenderer spriteRenderer;

        // || Properties

        public int MaxHits { get; set; } = 0;
        public int StartMaxHits { get; set; } = 0;
        public Color32 ParticlesColor { get; set; }
        public BoxCollider2D BoxCollider2D { get; private set; }

        private void Awake() => GetRequiredComponents();

        public void Start()
        {
            CountBreakableBlocks();

            MaxHits = (hasRandomHits ? (int)UnityEngine.Random.Range(1, hitSprites.Length + 2) : hitSprites.Length + 1);
            StartMaxHits = MaxHits;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (GameSessionController.Instance.ActualGameState == Enumerators.GameStates.GAMEPLAY)
            {
                if (!lastCollision)
                {
                    collidedWithBall = (other.gameObject.GetComponent<Ball>() != null);

                    if (CompareTag(NamesTags.Tags.BreakableBlock))
                    {
                        HandleHit();
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (GameSessionController.Instance.ActualGameState == Enumerators.GameStates.GAMEPLAY)
            {
                if (!lastCollision)
                {
                    collidedWithBall = (other.gameObject.GetComponent<Ball>() != null);

                    if (CompareTag(NamesTags.Tags.BreakableBlock))
                    {
                        HandleHit();
                    }
                }
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
                BoxCollider2D = GetComponent<BoxCollider2D>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetColor(Color32 color) => spriteRenderer.color = ParticlesColor = color;

        /// <summary>
        /// Count number of breakable blocks
        /// </summary>
        private void CountBreakableBlocks()
        {
            if (CompareTag(NamesTags.Tags.BreakableBlock))
            {
                GameSessionController.Instance.CountBlocks();
            }
        }

        /// <summary>
        /// Handle collision with ball
        /// </summary>
        private void HandleHit()
        {
            timesHit++;

            if (timesHit >= MaxHits)
            {
                DestroyBlock();
                lastCollision = true;
            }
            else
            {
                ShowNextSprite();
            }
        }

        /// <summary>
        /// Show block next sprite
        /// </summary>
        private void ShowNextSprite()
        {
            int spriteIndex = (timesHit - 1);
            if (hitSprites[spriteIndex] != null)
            {
                spriteRenderer.sprite = hitSprites[spriteIndex];
            }

            AudioClip clip = (isBallBig ? AudioController.Instance.HittingFaceSound : AudioController.Instance.SlamSound);
            AudioController.Instance.PlaySFX(clip, AudioController.Instance.MaxSFXVolume / 2f);
            SpawnDebris();
        }

        /// <summary>
        /// Destroy this block
        /// </summary>
        private void DestroyBlock()
        {
            if (collidedWithBall)
            {
                GameSessionController.Instance.AddToComboMultiplier();
            }

            int comboMultiplier = GameSessionController.Instance.ComboMultiplier;
            int score = (int)UnityEngine.Random.Range(minMaxPointsScore.x, minMaxPointsScore.y);
            score *= MaxHits;

            if (collidedWithBall)
            {
                if (comboMultiplier > 1)
                {
                    score *= comboMultiplier;
                }
            }

            TriggerExplosion();
            SpawnDebris();
            ShowScoreText(score);
            GameSessionController.Instance.AddToScore(score);

            if (spawnPowerUp)
            {
                SpawnPowerUp();
            }

            StartCoroutine(DestroyCoroutine());
        }

        /// <summary>
        /// Applies delay before Block destruction
        /// </summary>
        private IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            Destroy(gameObject);
            GameSessionController.Instance.BlockDestroyed();

            if (BlockGrid.CheckPosition(transform.position))
            {
                BlockGrid.RedefineBlock(transform.position, null);
            }
        }

        /// <summary>
        /// Trigger explosion animation
        /// </summary>
        private void TriggerExplosion()
        {
            try
            {
                if (explosionPrefabs.Length >= 1)
                {
                    AudioController.Instance.PlaySoundAtPoint(AudioController.Instance.ExplosionSound, AudioController.Instance.MaxSFXVolume / 2);
                    int randomIndex = UnityEngine.Random.Range(0, explosionPrefabs.Length);
                    GameObject explosion = Instantiate(explosionPrefabs[randomIndex], transform.position, Quaternion.identity) as GameObject;
                    explosion.transform.SetParent(GameSessionController.Instance.FindOrCreateObjectParent(NamesTags.Parents.Explosions).transform);
                    Animator animator = explosion.GetComponent<Animator>();
                    float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
                    Destroy(explosion, animationLength);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Show score text
        /// </summary>
        /// <param name="score"> Score value </param>
        private void ShowScoreText(int score)
        {
            try
            {
                TextMeshPro textMeshPro = blockScoreTextPrefab.GetComponentInChildren<TextMeshPro>();
                textMeshPro.text = Formatter.FormatToCurrency(score);
                GameObject scoreText = Instantiate(blockScoreTextPrefab, transform.position, Quaternion.identity) as GameObject;
                scoreText.transform.SetParent(GameSessionController.Instance.FindOrCreateObjectParent(NamesTags.Parents.BlockScoreText).transform);
                Animator animator = scoreText.GetComponent<Animator>();
                float durationLength = animator.GetCurrentAnimatorStateInfo(0).length;
                Destroy(scoreText, durationLength);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Spawn collision debris
        /// </summary>
        private void SpawnDebris()
        {
            try
            {
                // Instantiate and Destroy
                GameObject debris = Instantiate(particlesPrefab, transform.position, particlesPrefab.transform.rotation) as GameObject;
                debris.transform.SetParent(GameSessionController.Instance.FindOrCreateObjectParent(NamesTags.Parents.Debris).transform);

                // Color
                ParticleSystem debrisParticleSystem = debris.GetComponent<ParticleSystem>();
                var mainModule = debrisParticleSystem.main;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(ParticlesColor);

                // Time to destroy
                ParticleSystem prefabParticleSystem = particlesPrefab.GetComponent<ParticleSystem>();
                float durationLength = (prefabParticleSystem.main.duration + prefabParticleSystem.main.startLifetime.constant);
                Destroy(debris, durationLength);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Instantiates random power up
        /// </summary>
        private void SpawnPowerUp()
        {
            try
            {
                int randomIndex = UnityEngine.Random.Range(0, powerUpPrefabs.Length);
                GameObject powerUp = Instantiate(powerUpPrefabs[randomIndex].gameObject, transform.position, Quaternion.identity) as GameObject;
                powerUp.transform.SetParent(GameSessionController.Instance.FindOrCreateObjectParent(NamesTags.Parents.PowerUps).transform);
                AudioController.Instance.PlaySoundAtPoint(AudioController.Instance.ShowUpSound, AudioController.Instance.MaxSFXVolume);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}