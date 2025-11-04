using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    public int health = 1000;
    public int movementSpeed = 5;
    public int detectionRange;
    public int attackRange;

    GameObject Player;

    bool playerDetected = false;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Player = GameObject.FindWithTag("Player");
    }

    void FixedUpdate()
    {
        if (health <= 0)
            Destroy(this.gameObject);

        float distanceToPlayer = Vector3.Distance(rb.position, Player.transform.position);

        if (distanceToPlayer <= detectionRange)
            playerDetected = true;
        
        if (playerDetected == true)
        {
            Vector3 direction = (Player.transform.position - rb.position).normalized;
            Vector3 newPosition = rb.position + direction * movementSpeed * Time.fixedDeltaTime;

            rb.MovePosition(newPosition);

            transform.LookAt(Player.transform.position);
        }

        if (Vector3.Distance(rb.position, Player.transform.position) <= attackRange)
        {
            // TODO: Make the Player Take Damage.
        }
    }

    public void TakeDamage(int Amt)
    {
        if (health - Amt <= 0)
        {
            Destroy(this.gameObject);
        }

        health -= Amt;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            //tries to find the bullet's PlayerBullet script, which contains information on how much damage it deals
            PlayerBullet bulletInfo = collision.gameObject.GetComponent<PlayerBullet>();

            //If this scipt found the bullet's script, it can deal the proper amount of damage. Otherwise it will just deal 10.
            if(bulletInfo != null)
            {
                TakeDamage(bulletInfo.damage);
            }
            else
            {
                TakeDamage(10);
            }

            //The bullet has served its purpose. May it share its glory in Valhalla.
            Destroy(collision.gameObject);
        }
    }
}
