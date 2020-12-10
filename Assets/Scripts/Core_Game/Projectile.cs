using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Config params
    private float maxForceY = 1000f;
    private float minForceY = 500f;
    private float timeToSelfDestruct = 2f;

    // Cached
    private Rigidbody2D rigidBody2D;

    private void Awake()
    {
        rigidBody2D = this.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Invoke("HideObject", 2f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(NamesTags.BreakableBlockTag) ||
            other.gameObject.CompareTag(NamesTags.UnbreakableBlockTag))
        {
            this.gameObject.SetActive(false);
        }
    }

    private void HideObject()
    {
        this.gameObject.SetActive(false);
    }

    // Add random force in Y to projectile
    public void MoveProjectile()
    {
        float randomForce = Random.Range(minForceY, maxForceY);
        Vector2 newForce = new Vector2(0, randomForce);
        if (!rigidBody2D)
        {
            rigidBody2D = this.GetComponent<Rigidbody2D>();
        }

        rigidBody2D.AddForce(newForce);
    }
}