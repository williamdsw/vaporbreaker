using UnityEngine;

public class EchoEffect : MonoBehaviour
{
    // Params Config
    [SerializeField] private GameObject echoPrefab;
    private float startTimeBetweenSpanws = 0.05f;
    private float timeBetweenSpawns = 0;
    private float timeToSelfDestruct = 1f;

    // Cached
    private Ball ball;
    private GameSession gameSession;
    private Paddle paddle;

    public void SetTimeToSelfDestruct(float time)
    {
        this.timeToSelfDestruct = time;
    }

    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        DefineReferences();
    }

    private void Update()
    {
        SpawnEchoEffect();
    }

    private void DefineReferences()
    {
        if (tag == NamesTags.GetBallEchoTag())
        {
            ball = this.transform.parent.GetComponent<Ball>();
        }
        else if (tag == NamesTags.GetPaddleEchoTag())
        {
            paddle = this.transform.parent.GetComponent<Paddle>();
        }
    }

    // Verify times and instantiate the prefab of echo
    private void SpawnEchoEffect()
    {
        if (timeBetweenSpawns <= 0)
        {
            GameObject echo = Instantiate(echoPrefab, transform.position, Quaternion.identity) as GameObject;
            echo.transform.parent = gameSession.FindOrCreateObjectParent(NamesTags.GetEchosParentName()).transform;
            if (tag == NamesTags.GetBallEchoTag() && ball)
            {
                echo.transform.localScale = ball.transform.localScale;
                echo.transform.rotation = ball.transform.rotation;
                SpriteRenderer spriteRenderer = echo.GetComponent<SpriteRenderer>();
                spriteRenderer.color = ball.GetBallColor();
            }
            else if (tag == NamesTags.GetPaddleEchoTag() && paddle)
            {
                SpriteRenderer spriteRenderer = echo.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = paddle.GetSprite();
            }

            Destroy(echo, timeToSelfDestruct);
            timeBetweenSpawns = startTimeBetweenSpanws;
        }
        else
        {
            timeBetweenSpawns -= Time.deltaTime;
        }
    }
}