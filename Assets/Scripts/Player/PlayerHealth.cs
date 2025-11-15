using System;
using UnityEngine;

[RequireComponent(typeof(PlayerLookControls))]
public class PlayerHealth : MonoBehaviour
{

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int health;

    [Header("Armor percent. 0 = no armor, 1 = all damage negated")]
    public float armorDamageReductionPercent = 0f;

    [Header("UI Refs")]
    public GameObject RestartScreen;

    public Action? HealthChanged;

    void Start()
    {
        health = maxHealth - 50;
    }

    public void TakeDamage(int amt)
    {
        amt = (int)(amt * (1 - armorDamageReductionPercent));

        health -= amt;
        HealthChanged?.Invoke();

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    public void Heal(int Amt)
    {
        health = Mathf.Clamp(health, health + Amt, maxHealth);
        HealthChanged.Invoke();
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
