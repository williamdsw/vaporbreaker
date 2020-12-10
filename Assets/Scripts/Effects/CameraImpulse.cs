using UnityEngine;

public class CameraImpulse : MonoBehaviour
{
    // Config
    private float movementSpeed = 5f;

    // State
    private bool canLerp = false;
    private static CameraImpulse instance;

    // Cached
    private Vector3 originalPosition;
    private Vector3 destinyPosition;

    public static CameraImpulse Instance
    {
        get => instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        originalPosition = this.transform.position;
        destinyPosition = new Vector3(originalPosition.x, 1f, originalPosition.z);
    }

    private void LateUpdate()
    {
        LerpCamera();
    }

    // Set initial camera impulse upwards
    public void TriggerImpulse()
    {
        if (canLerp) return;
        this.transform.position = destinyPosition;
        canLerp = true;
    }

    // Lerp between current position and original position
    private void LerpCamera()
    {
        if (canLerp)
        {
            if (this.transform.position == originalPosition || this.transform.position.y <= 0.1f)
            {
                this.transform.position = originalPosition;
                canLerp = false;
                return;
            }

            this.transform.position = Vector3.Lerp(this.transform.position, originalPosition, movementSpeed * Time.deltaTime);
        }
    }
}