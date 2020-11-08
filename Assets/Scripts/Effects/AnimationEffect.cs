using UnityEngine;
using UnityEngine.UI;

public class AnimationEffect : MonoBehaviour
{
    // Config
    [SerializeField] private Sprite[] spritesToAnimateList;
    [SerializeField] private bool randomFPS = false;
    [SerializeField] private float framesPerSecond = 10;

    // Cached Components
    private Image image;
    private SpriteRenderer spriteRenderer;

    //--------------------------------------------------------------------------------//

    private void Start () 
    {
        // Get components
        image = this.GetComponent<Image>();

        if (!image) { spriteRenderer = this.GetComponent<SpriteRenderer>(); }

        // Chooses random FPS
        framesPerSecond = (randomFPS ? Random.Range (5, 30) : framesPerSecond);
    }

    private void FixedUpdate () 
    {
        AnimateImage ();
    }

    //--------------------------------------------------------------------------------//

    private void AnimateImage ()
    {
        //Cancel
        if (spritesToAnimateList.Length == 0) { return; }

        int index = (int) (Time.fixedTime * framesPerSecond) % spritesToAnimateList.Length;

        if (image) { image.sprite = spritesToAnimateList[index]; }
        else { spriteRenderer.sprite = spritesToAnimateList[index]; }
    }
}