using Assets.Scripts;
using UnityEngine;

public class PlayerGrenade : MonoBehaviour
{
    public Rigidbody rb;
    public float launchForce = 50f;
    public float fuseTime = 5f;
    public Vector2 randomRotationRange;

    public GameObject explosionObject;

    public PlayerWeaponManager? Owner { get; set; }

    void Start()
    {
        rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);
        rb.angularVelocity = new Vector3(Random.Range(randomRotationRange.x, randomRotationRange.y), Random.Range(randomRotationRange.x, randomRotationRange.y), Random.Range(randomRotationRange.x, randomRotationRange.y));
        Invoke("Explode", fuseTime);
    }

   public void Explode()
    {
        if(explosionObject != null)
        {
            GameObject ex = Instantiate(explosionObject, transform.position, transform.rotation);
            if (ex.GetComponent<PlayerBullet>() != null)
                ex.GetComponent<PlayerBullet>().Owner = Owner;
        }
        Destroy(gameObject);
    }
}
