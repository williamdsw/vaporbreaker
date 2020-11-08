using UnityEngine;

public class LineRendererScroller : MonoBehaviour
{
    // Config
    private float xMovementSpeed = - 5f;
    private float yMovementSpeed = 0f;

    // Cached
    private Material material;
    private LineRenderer lineRenderer;
    private Vector2 offset;

    //--------------------------------------------------------------------------------//

    private void Start ()
    {
        // Components
        lineRenderer = this.GetComponent<LineRenderer>();

        material = lineRenderer.material;
        offset = new Vector2 (xMovementSpeed, yMovementSpeed);
    }

    private void FixedUpdate ()
    {
        material.mainTextureOffset += offset * Time.fixedDeltaTime;
    }
}