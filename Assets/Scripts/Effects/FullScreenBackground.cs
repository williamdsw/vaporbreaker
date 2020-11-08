using UnityEngine;

public class FullScreenBackground : MonoBehaviour
{
    // Config
    [SerializeField] private Sprite[] backgroundSprites;

    // Cached
    private SpriteRenderer spriteRenderer;

    //--------------------------------------------------------------------------------//

    private void Awake () 
    {
        SetupSingleton ();
    }

    private void Start ()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        ChooseRandomSprite ();
        CalculateSpriteScale ();
    }

    //--------------------------------------------------------------------------------//

    private void SetupSingleton ()
    {
        int numberOfInstances = FindObjectsOfType (GetType ()).Length;
        if (numberOfInstances > 1)
        {
            Destroy (this.gameObject);
        }
        else 
        {
            DontDestroyOnLoad (this.gameObject);
        }
    }

    public void DestroyInstance ()
    {
        Destroy (this.gameObject);
    }

    //--------------------------------------------------------------------------------//

    // Choose an random sprite for background
    private void ChooseRandomSprite ()
    {
        // Load and verify
        if (backgroundSprites.Length == 0) { return; }

        int index = Random.Range (0, backgroundSprites.Length);
        spriteRenderer.sprite = backgroundSprites[index];
    }

    // Calculates sprite scale to fit camera
    private void CalculateSpriteScale ()
    {
        Camera mainCamera = Camera.main;

        // Sizes
        float cameraHeight = mainCamera.orthographicSize * 2;
        Vector2 cameraSize = new Vector2 (Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // Calculate new Scale
        Vector2 newScale = transform.localScale;
        if (cameraSize.x >= cameraSize.y)
        {
            newScale *= cameraSize.x / spriteSize.x;
        }
        else
        {
            newScale *= cameraSize.y / spriteSize.y;
        }

        transform.position = Vector2.zero;
        transform.localScale = newScale;
    }
}