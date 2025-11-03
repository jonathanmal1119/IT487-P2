using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour
{
    public int health = 1000;
    public int movementSpeed = 5;
    public int detectionRange;

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
    }
}
