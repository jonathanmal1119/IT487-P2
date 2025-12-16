using UnityEngine;

public class MoveParentForward : MonoBehaviour
{
    public float speed = 5f;

    [Header("Pan Up Settings")]
    public float panSpeed = 5f;      // degrees per second
    public float maxPanAngle = 20f;   // max upward tilt

    private float currentPan = 0f;

    void Update()
    {
        // Move forward
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);

        // Slow pan up
        if (currentPan < maxPanAngle)
        {
            float panThisFrame = panSpeed * Time.deltaTime;
            currentPan += panThisFrame;

            transform.Rotate(Vector3.right, -panThisFrame, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1000);
        }
    }
}
