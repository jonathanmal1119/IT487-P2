using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public Rigidbody rb;
    public float launchForce = 50f;
    public int damage = 1;
    public float lifetime = 5f;
    public bool destroyOnHit = true;
    public bool ignoreGround = false;

    public bool waitingToDestroy = false;

    void Start()
    {
        rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogWarning("TODO: implement player bullets damaging enemies on hit");
        if (other.CompareTag("Enemy"))
        {
            //Debug.LogWarning("TODO: implement player bullets damaging enemies on hit");
        }
        else if(ignoreGround == false && other.gameObject.layer == 0 && other.CompareTag("Player") == false)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }
}
