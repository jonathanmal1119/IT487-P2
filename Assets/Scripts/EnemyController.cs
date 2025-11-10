using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    public int health = 1000;
    public int movementSpeed = 5;
    public int detectionRange;
    public int attackRange = 3;
    public int attackAmt = 10;
    public float attackTime = 1.5f;

    GameObject Player;

    bool playerDetected = false;
    bool attacking = false;

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

        if (Vector3.Distance(rb.position, Player.transform.position) <= attackRange && !attacking)
        {
            StartCoroutine(DamageCoroutine());
        }
    }

    IEnumerator DamageCoroutine()
    {
        attacking = true;

        while (Vector3.Distance(rb.position, Player.transform.position) <= attackRange)
        {
            Player.GetComponent<PlayerHealth>().TakeDamage(attackAmt);
            yield return new WaitForSeconds(attackTime);
        }

        attacking = false;
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
            if(bulletInfo != null && bulletInfo.waitingToDestroy == false)
            {
                TakeDamage(bulletInfo.damage);
            }
            else
            {
                TakeDamage(10);
            }

            if(bulletInfo != null && bulletInfo.destroyOnHit)
            {
                Destroy(collision.gameObject);
                bulletInfo.waitingToDestroy = true;
            }
        }

        if (collision.gameObject.tag == "Vehicle")
        {
            TakeDamage(100);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            //tries to find the bullet's PlayerBullet script, which contains information on how much damage it deals
            PlayerBullet bulletInfo = other.gameObject.GetComponent<PlayerBullet>();

            //If this scipt found the bullet's script, it can deal the proper amount of damage. Otherwise it will just deal 10.
            if (bulletInfo != null && bulletInfo.waitingToDestroy == false)
            {
                TakeDamage(bulletInfo.damage);
            }
            else
            {
                TakeDamage(10);
            }

            if (bulletInfo != null && bulletInfo.destroyOnHit)
            {
                Destroy(other.gameObject);
                bulletInfo.waitingToDestroy = true;
            }
        }

        if (other.gameObject.tag == "Vehicle")
        {
            TakeDamage(100);
        }
    }
}
