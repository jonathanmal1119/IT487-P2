using Assets.Scripts;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UI.GridLayoutGroup;

public class EnemyController : MonoBehaviour
{
    public int health = 1000;
    public float movementSpeed = 5;
    public float idleSpeed = 2;
    public int detectionRange;
    public int attackRange = 3;
    public int attackAmt = 10;
    public float attackTime = 1.5f;
    public bool isDead = false;

    GameObject Player;
    public AudioSource audio;
    public AudioClip growl;

    public Animator animator;
    public RuntimeAnimatorController walk;
    public RuntimeAnimatorController death;
    public RuntimeAnimatorController attack;

    public bool playerDetected = false;
    bool attacking = false;

    public GameObject[] drops;

    float distanceToPlayer;

    bool lostPlayer = false;

    float randDirectionX = 0f;
    float randDirectionZ = 0f;
    bool isChangingDirection = false;

    Rigidbody rb;

    bool isPlayingSound = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Player = GameObject.FindWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        audio = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        Vector3 direction;
        Vector3 newPosition;
        Quaternion rotation;

        if (!isDead)
        {
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
                direction = (Player.transform.position - rb.position).normalized;
                newPosition = rb.position + direction * movementSpeed * Time.fixedDeltaTime;

                rb.MovePosition(newPosition);

                transform.LookAt(Player.transform.position);
            }

            if (Vector3.Distance(rb.position, Player.transform.position) <= attackRange && !attacking)
            {
                attacking = true;
                StartCoroutine(DamageCoroutine());
            }

            if (playerDetected == false)
            {
                direction = new Vector3(randDirectionX, 0, randDirectionZ);
                newPosition = rb.position + direction * idleSpeed * Time.fixedDeltaTime;

                if (direction != Vector3.zero)
                {
                    rotation = Quaternion.LookRotation(direction);
                    rb.MoveRotation(rotation);
                }

                rb.MovePosition(newPosition);
                if (isChangingDirection == false)
                {
                    StartCoroutine(changeDirection());
                }
            }

            if (this.rb.position.y < -100)
                StartCoroutine(Death());

            if (isPlayingSound == false && audio != null)
            {
                isPlayingSound = true;
                StartCoroutine(playSound());
            }
        }
    }

    IEnumerator playSound()
    {
        int wait = Random.Range(0, 10);
        audio.clip = growl;
        audio.Play();
        yield return new WaitForSeconds(wait);
        isPlayingSound = false;
    }

    IEnumerator DamageCoroutine()
    {
        while (Vector3.Distance(rb.position, Player.transform.position) <= attackRange)
        {
            Player.GetComponent<PlayerHealth>().TakeDamage(attackAmt);
            animator.runtimeAnimatorController = attack;
            yield return new WaitForSeconds(attackTime);
        }

        animator.runtimeAnimatorController = walk;
        attacking = false;
    }

    IEnumerator Death()
    {
        animator.runtimeAnimatorController = death;
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }

    public void TakeDamage(int Amt, out bool killed)
    {
        if (health - Amt <= 0)
        {
            int dropChance = Random.Range(0, 2);
            if (drops.Length > 0 && dropChance == 1)
            {
                int selection = Random.Range(0, drops.Length);
                if (drops[selection] != null)
                    Instantiate(drops[selection], rb.transform.position, transform.rotation);
            }
            
            isDead = true;
            StartCoroutine(Death());
        }

        health -= Amt;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            playerDetected = true;

            //tries to find the bullet's PlayerBullet script, which contains information on how much damage it deals
            PlayerBullet? bulletInfo = collision.gameObject.GetComponent<PlayerBullet>().N();

            //If this scipt found the bullet's script, it can deal the proper amount of damage. Otherwise it will just deal 10.
            if (bulletInfo?.waitingToDestroy == false)
            {
                TakeDamage(bulletInfo.damage, out bool killed);
                if (killed)
                    bulletInfo?.Owner?.OnKill?.Invoke();
                else
                    bulletInfo?.Owner?.OnHit?.Invoke();
            }
            else
            {
                TakeDamage(10, out _);
            }

            if (bulletInfo?.destroyOnHit == true)
            {
                bulletInfo.DisableBullet();

                //Destroy(collision.gameObject);
                bulletInfo.waitingToDestroy = true;
            }
        }

        if (collision.gameObject.tag == "Vehicle")
        {
            TakeDamage(100, out _);
        }
    }


    //Enemy loses focus on Player when player is far enough away
    private IEnumerator outsideDetRad()
    {
            yield return new WaitForSeconds(7f);
            playerDetected = false;
    }

    private IEnumerator changeDirection()
    {
        isChangingDirection = true;

        int delay = Random.Range(0, 7);
        yield return new WaitForSeconds(delay);
        randDirectionX = Random.Range(-2, 2);
        randDirectionZ = Random.Range(-2, 2);

        isChangingDirection = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            playerDetected = true;

            //tries to find the bullet's PlayerBullet script, which contains information on how much damage it deals
            PlayerBullet? bulletInfo = other.gameObject.GetComponent<PlayerBullet>().N();

            //If this scipt found the bullet's script, it can deal the proper amount of damage. Otherwise it will just deal 10.
            if (bulletInfo?.waitingToDestroy == false)
            {
                TakeDamage(bulletInfo.damage, out bool killed);
                if (killed)
                    bulletInfo?.Owner?.OnKill?.Invoke();
                else
                    bulletInfo?.Owner?.OnHit?.Invoke();
            }
            else
            {
                TakeDamage(10, out _);
            }

            if (bulletInfo?.destroyOnHit == true)
            {
                bulletInfo.DisableBullet();

                //Destroy(other.gameObject);
                bulletInfo.waitingToDestroy = true;
            }
        }
    }
}
