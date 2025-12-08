using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerBullet : MonoBehaviour
{
    public Rigidbody rb;
    public float launchForce = 50f;
    public int damage = 1;
    public float lifetime = 5f;
    public bool destroyOnHit = true;
    public bool ignoreGround = false;

    public Vector3 PlayerHitScreenshake = Vector3.zero;

    public GameObject solidImpactFX;
    public GameObject enemyImpactFX;

    public bool waitingToDestroy = false;

    public PlayerWeaponManager? Owner { get; set; }

    void Start()
    {
        rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);
        
        Invoke(nameof(DisableBullet), lifetime); // disable bullet based on lifetime
        Destroy(gameObject, lifetime + 5); // keep object alive after disabling so the vfx can finish playing before destroying
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogWarning("TODO: implement player bullets damaging enemies on hit");
        if (other.CompareTag("Enemy"))
        {
            //Debug.LogWarning("TODO: implement player bullets damaging enemies on hit");
            //Owner?.OnHit?.Invoke();
            if (enemyImpactFX != null)
            {
                Instantiate(enemyImpactFX, transform.position, transform.rotation);
            }
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerLookControls>().N()?.AddScreenShake(PlayerHitScreenshake.x, PlayerHitScreenshake.y, PlayerHitScreenshake.z);
        }
        else if(ignoreGround == false && other.gameObject.layer == 0 && other.CompareTag("Player") == false && other.CompareTag("Bullet") == false)
        {
            // keeping it alive lets the trail disappear normally instead of abruptly disappearing
            DisableBullet();
            if (solidImpactFX != null)
            {
                Instantiate(solidImpactFX, transform.position, transform.rotation);
            }
        }
    }
    private void OnCollisionEnter(Collision collision) => OnTriggerEnter(collision.collider);

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
