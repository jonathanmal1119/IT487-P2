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
        //Destroy(gameObject, lifetime);

        Invoke(nameof(DisableBullet), lifetime); // disable bullet based on lifetime
        Destroy(gameObject, lifetime + 5); // keep object alive after disabling so the vfx can finish playing before destroying
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    public void DisableBullet()
    {
        GetComponent<Collider>().enabled = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            meshRenderer.enabled = false;
        foreach (ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>())
            particleSystem.Stop();
    }
}
