using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int health;

    [Header("UI Refs")]
    public Text HealthUI;
    public GameObject RestartScreen;

    void Start()
    {
        health = maxHealth - 50;
        HealthUI.text = "HP: " + health.ToString();
    }

    public void TakeDamage(int Amt)
    {
       if (health - Amt <= 0)
        {
            Die();
            return;
        }
            
        health -= Amt;
        HealthUI.text = "HP: " + health.ToString();
    }

    public void Heal(int Amt)
    {
        health = Mathf.Clamp(health, health + Amt, maxHealth);
        HealthUI.text = "HP: " + health.ToString();
    }

    void Die()
    {
        Debug.Log("Player Died");
        GetComponent<PlayerLookControls>().enabled = false;
        RestartScreen.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyBullet")
        {
            Debug.Log("Player hit by enemy bullet");
            EnemyBullet bulletInfo = collision.gameObject.GetComponent<EnemyBullet>();

            if (bulletInfo != null)
            {
                TakeDamage(bulletInfo.damage);
            }
            else
            {
                TakeDamage(10);
            }

            Destroy(collision.gameObject);
        }
    }
}
