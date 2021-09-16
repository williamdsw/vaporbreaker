using Controllers.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

namespace Core
{
    public enum BlockColors
    {
        Artic, Blue, Brown, Butter, Cerulean, Dark_Green, Green, Lilac, Ocean, Olive,
        Orange, Periwinkle, Pine, Pink, Purple, Red, Ruby, Silver, Strawberry, Teal, Yellow
    }

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
        [SerializeField] private BlockColors blockColor;
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

        private Color32 particlesColor;
        private SpriteRenderer spriteRenderer;

        // || Properties

        public int MaxHits { get; set; } = 0;
        public int StartMaxHits { get; set; } = 0;
        public Color32 ParticlesColor { get => particlesColor; set => particlesColor = value; }
        public BlockColors BlockColor { get => blockColor; set => blockColor = value; }

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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetColor(Color32 color) => spriteRenderer.color = particlesColor = color;

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

        public void DefineParticlesColor(BlockColors blockColor)
        {
            switch (blockColor)
            {
                case BlockColors.Artic: { ParticlesColor = new Color32(23, 255, 255, 255); break; }
                case BlockColors.Blue: { ParticlesColor = Color.blue; break; }
                case BlockColors.Brown: { ParticlesColor = new Color32(144, 88, 64, 255); break; }
                case BlockColors.Butter: { ParticlesColor = new Color32(222, 148, 99, 255); break; }
                case BlockColors.Cerulean: { ParticlesColor = new Color32(66, 165, 255, 255); break; }
                case BlockColors.Dark_Green: { ParticlesColor = new Color32(0, 64, 32, 255); break; }
                case BlockColors.Green: { ParticlesColor = Color.green; break; }
                case BlockColors.Lilac: { ParticlesColor = new Color32(205, 128, 209, 255); break; }
                case BlockColors.Ocean: { ParticlesColor = new Color32(0, 214, 181, 255); break; }
                case BlockColors.Olive: { ParticlesColor = new Color32(139, 173, 15, 255); break; }
                case BlockColors.Orange: { ParticlesColor = new Color32(255, 107, 0, 255); break; }
                case BlockColors.Periwinkle: { ParticlesColor = new Color32(177, 108, 248, 255); break; }
                case BlockColors.Pine: { ParticlesColor = new Color32(0, 96, 96, 255); break; }
                case BlockColors.Pink: { ParticlesColor = new Color32(208, 47, 216, 255); break; }
                case BlockColors.Purple: { ParticlesColor = new Color32(74, 16, 107, 255); break; }
                case BlockColors.Red: { ParticlesColor = Color.red; break; }
                case BlockColors.Ruby: { ParticlesColor = new Color32(64, 0, 0, 255); break; }
                case BlockColors.Silver: { ParticlesColor = Color.gray; break; }
                case BlockColors.Strawberry: { ParticlesColor = new Color32(198, 49, 82, 255); break; }
                case BlockColors.Teal: { ParticlesColor = new Color32(91, 255, 255, 255); break; }
                case BlockColors.Yellow: { ParticlesColor = Color.yellow; break; }
                default: { ParticlesColor = Color.white; break; }
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
                    AudioController.Instance.PlaySFX(AudioController.Instance.ExplosionSound, AudioController.Instance.MaxSFXVolume / 2);
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
                if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

                int randomIndex = UnityEngine.Random.Range(0, powerUpPrefabs.Length);
                GameObject powerUp = Instantiate(powerUpPrefabs[randomIndex].gameObject, transform.position, Quaternion.identity) as GameObject;
                powerUp.transform.SetParent(GameSessionController.Instance.FindOrCreateObjectParent(NamesTags.Parents.PowerUps).transform);
                AudioController.Instance.PlaySFX(AudioController.Instance.ShowUpSound, AudioController.Instance.MaxSFXVolume);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}