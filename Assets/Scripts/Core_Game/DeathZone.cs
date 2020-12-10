using System.Collections;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // Cached Components
    private EdgeCollider2D edgeCollider;

    // Cached Objects
    private AudioController audioController;
    private Ball[] balls;
    private FadeEffect fadeEffect;
    private GameSession gameSession;

    private void Awake() 
    {
        edgeCollider = this.GetComponent<EdgeCollider2D>();
    }

    private void Start ()
    {
        audioController = FindObjectOfType<AudioController>();
        fadeEffect = FindObjectOfType<FadeEffect>();
        gameSession = FindObjectOfType<GameSession>();

        DefineColliderPoints ();
    }

    private void OnCollisionEnter2D (Collision2D other)
    {
        if (gameSession.GetActualGameState () == GameState.GAMEPLAY)
        {
            if (other.gameObject.CompareTag (NamesTags.GetBallTag ()))
            {
                DealWithBallCollision (other.gameObject);
            }
            else if (other.gameObject.CompareTag (NamesTags.GetPowerUpTag ()))
            {
                Destroy (other.gameObject);
            }
        }
    }

    // Defines the collider points based on size of screen
    private void DefineColliderPoints ()
    {
        Camera mainCamera = Camera.main;
        Vector2 lowerLeftCorner = mainCamera.ScreenToWorldPoint (new Vector3 (0, 0, 0));
        Vector2 lowerRightCorner = mainCamera.ScreenToWorldPoint (new Vector3 (Screen.width, 0, 0));
        lowerLeftCorner.x = Mathf.FloorToInt (lowerLeftCorner.x) * 2;
        lowerRightCorner.x = Mathf.CeilToInt (lowerRightCorner.x) * 2;
        edgeCollider.points = new Vector2[] { lowerLeftCorner, lowerRightCorner };
    }

    // Deals with collision with ball depending on how much balls are on screen
    private void DealWithBallCollision (GameObject otherBall)
    {
        balls = FindObjectsOfType<Ball>();

        if (balls.Length == 1)
        {
            StartCoroutine (WaitToReset ());
        }
        else 
        {
            audioController.PlaySFX (audioController.BoomSound, 0.3f);
            gameSession.CurrentNumberOfLaunchedBalls--;
        }

        Destroy (otherBall);
    }

    // Plays SFX and wait to call fade out
    private IEnumerator WaitToReset () 
    {
        // Other
        gameSession.SetActualGameState (GameState.TRANSITION);
        float soundLength = audioController.GetClipLength (audioController.BoomSound);
        audioController.PlaySFX (audioController.BoomSound, 0.8f);
        yield return new WaitForSecondsRealtime (soundLength);
        fadeEffect.FadeToLevel ();
    }
}