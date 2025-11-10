using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public Rigidbody rb;
    public float launchForce = 50f;
    public int damage = 1;
    public float lifetime = 5f;

    void Start()
    {
        rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }
}
