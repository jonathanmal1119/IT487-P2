using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    public int health = 1000;
    public float movementSpeed = 5;
    public int detectionRange;
    public int attackRange = 3;
    public int attackAmt = 10;
    public float attackTime = 1.5f;

    GameObject Player;

    public bool playerDetected = false;
    bool attacking = false;

    public GameObject[] drops;

    float distanceToPlayer;

    bool lostPlayer = false;

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

        distanceToPlayer = Vector3.Distance(rb.position, Player.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
            lostPlayer = false;
        }

        if (playerDetected == true && distanceToPlayer > detectionRange && lostPlayer == false)
        {
            StartCoroutine(outsideDetRad());
            lostPlayer = true;
        }

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
            int dropChance = Random.Range(0, 2);
            if (drops.Length > 0 && dropChance == 1)
            {
                int selection = Random.Range(0, drops.Length);
                if (drops[selection] != null)
                    Instantiate(drops[selection], rb.transform.position, this.transform.rotation);
            }
            
            Destroy(this.gameObject);
        }

        health -= Amt;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            playerDetected = true;

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

        if (collision.gameObject.tag == "Vehicle")
        {
            TakeDamage(100);
        }
    }


    //Enemy loses focus on Player when player is far enough away
    private IEnumerator outsideDetRad()
    {
            yield return new WaitForSeconds(7f);
            playerDetected = false;
    }
}
