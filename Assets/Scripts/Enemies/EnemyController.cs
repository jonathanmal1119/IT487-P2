using Assets.Scripts;
using System.Collections;
//using Unity.VisualScripting;

//using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
//using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;
//using UnityEngine.WSA;
//using UnityEngine.Audio;
//using UnityEngine.Rendering;
//using static UnityEngine.UI.GridLayoutGroup;

public class EnemyController : MonoBehaviour
{
    public bool isRangedEnemy;

    public float health = 100;
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
    public RuntimeAnimatorController takeDamage;
    public RuntimeAnimatorController idle;
    public RuntimeAnimatorController aim;
    bool isFlinching = false;

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

    public Slider HealthSlider;

    public GameObject head;

    public GameObject model;
    public GameObject bodyExplode;

    public bool returningToStart = false;
    Vector3 Startpos;

    public GameObject HeadshotEffect;
    public bool isHeadshot = false;
    public AudioClip headshotSoundEffect;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Player = GameObject.FindWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        audio = GetComponent<AudioSource>();
        Startpos = this.transform.position;
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
                if (isRangedEnemy)
                    animator.runtimeAnimatorController = aim;

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



            if (!playerDetected)
            {
                float wanderDistance = Vector3.Distance(rb.position, Startpos);

                if (!returningToStart)
                {
                    direction = new Vector3(randDirectionX, 0, randDirectionZ);

                    rotation = Quaternion.LookRotation(direction);
                    rb.MoveRotation(rotation);

                    newPosition = rb.position + direction * idleSpeed * Time.fixedDeltaTime;

                    rb.MovePosition(newPosition);
                }
                
                if (wanderDistance >= 10 && returningToStart == false)
                {
                    //returningToStart = true;

                    direction = (Startpos - transform.position).normalized;

                    transform.LookAt(direction);

                    newPosition = rb.position + direction * idleSpeed * Time.fixedDeltaTime;

                    rb.MovePosition(newPosition);                    
                }

                if (Vector3.Distance(rb.position, Startpos) < 2)
                {
                    returningToStart = false;
                }

                if (isChangingDirection == false)
                {
                    StartCoroutine(changeDirection());
                }
            }

            /*if (!playerDetected && Vector3.Distance(Startpos, this.transform.position) >= 10 && !returningToStart)
            {
                returningToStart = true;
                direction = (Startpos - this.transform.position).normalized;
                rotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(rotation);
                newPosition = rb.position + direction * idleSpeed * Time.fixedDeltaTime;

                rb.MovePosition(newPosition);
            }

            if (Vector3.Distance(this.transform.position, Startpos) < 9)
                returningToStart = false;*/

            if (this.rb.position.y < -100)
                StartCoroutine(Death());

            if (isPlayingSound == false && audio != null)
            {
                isPlayingSound = true;
                StartCoroutine(playSound());
            }

            /*if (rb.angularVelocity == new Vector3(0, 0, 0))
                animator.runtimeAnimatorController = idle;
            else
                animator.runtimeAnimatorController = walk;*/
        }
        if (HealthSlider != null)
            HealthSlider.value = health;

        HealthSlider.transform.LookAt(Player.transform.position);
    }
    IEnumerator playSound()
    {
        int wait = Random.Range(0, 10);
        audio.clip = growl;
        audio.Play();
        yield return new WaitForSeconds(wait);
        isPlayingSound = false;
    }

    IEnumerator lowerVolume()
    {
        while (audio.volume > 0)
        {
            audio.volume -= 0.01f;
            yield return new WaitForSeconds(0.015f);
        }
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
        if (isHeadshot == true)
        {
            Instantiate(HeadshotEffect, head.transform.position, transform.rotation);
            audio.clip = headshotSoundEffect;
            audio.Play();
            StartCoroutine(lowerVolume());
            Debug.Log("HeadSHOT KILL");
        }
        animator.runtimeAnimatorController = death;
        yield return new WaitForSeconds(0.8f);
        model.SetActive(false);
        bodyExplode.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }

    IEnumerator flinch()
    {
        animator.runtimeAnimatorController = takeDamage;
        yield return new WaitForSeconds(1f);
        if (health > 0)
            animator.runtimeAnimatorController = walk;
    }

    public void TakeDamage(float Amt, out bool killed)
    {
        killed = false;

        if (!killed)
            StartCoroutine(flinch());

        if (health - Amt <= 0)
        {
            int dropChance = Random.Range(0, 2);
            if (drops.Length > 0 && dropChance == 1)
            {
                int selection = Random.Range(0, drops.Length);
                if (drops[selection] != null)
                    Instantiate(drops[selection], rb.transform.position, transform.rotation);
            }
            

            if (health > 0)
                killed = true;

            isDead = true;
            StartCoroutine(Death());
        }

        if (!killed)
            isHeadshot = false;
        
        health -= Amt;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            playerDetected = true;

            //tries to find the bullet's PlayerBullet script, which contains information on how much damage it deals
            PlayerBullet? bulletInfo = collision.gameObject.GetComponent<PlayerBullet>().N();

            float distanceToHead = Vector3.Distance(bulletInfo.transform.position, head.transform.position);
            float damage = bulletInfo.damage;

            //If this scipt found the bullet's script, it can deal the proper amount of damage. Otherwise it will just deal 10.
            if (bulletInfo?.waitingToDestroy == false)
            {
                if (distanceToHead <= 0.5)
                {
                    Debug.Log("HEADSHOT");
                    damage = bulletInfo.damage * 2.0f;
                    isHeadshot = true;
                }
                else
                {
                    damage = bulletInfo.damage;
                }

                TakeDamage(damage, out bool killed);

                // show kill marker if hit was a kill, otherwise show hit marker if still alive
                if (killed)
                    bulletInfo?.Owner?.OnKill?.Invoke();
                else if (!isDead)
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
            if (!collision.gameObject.GetComponent<VehicleController>().isPlayerInCar)
                return;

            TakeDamage(100, out _);
            StartCoroutine(launchCorpse());
        }
    }

    IEnumerator launchCorpse()
    {
        Vector3 launchPos = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z);

        while (this.transform.position != launchPos)
        {
            Vector3 launchDirection = new Vector3(0, 1, 0);
            Vector3 launch = rb.position + launchDirection * movementSpeed * Time.deltaTime;
            rb.MovePosition(launch);
            yield return new WaitForSeconds(0.01f);
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
        do {
            randDirectionX = Random.Range(-2, 2);
            randDirectionZ = Random.Range(-2, 2);
        } while (randDirectionX == 0 && randDirectionZ == 0);

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

                // show kill marker if hit was a kill, otherwise show hit marker if still alive
                if (killed)
                    bulletInfo?.Owner?.OnKill?.Invoke();
                else if (!isDead)
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
