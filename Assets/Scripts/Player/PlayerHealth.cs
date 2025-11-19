using System;
using UnityEngine;

[RequireComponent(typeof(PlayerLookControls))]
public class PlayerHealth : MonoBehaviour
{

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int health;

    [Header("Demage negation percent. 0 = take all damage, 1 = all damage negated")]
    public float armorDamageReductionPercent = 0f;
    [Header("Armor as a secondary health value. Absorbs all damage so long as it is greater than zero.")]
    public int armorPoints = 0;

    [Header("UI Refs")]
    public GameObject RestartScreen;

    public Action? HealthChanged;

    public bool IsInvincible => Time.time - lastHitTime < 0.5f;
    private float lastHitTime = float.MinValue;

    public bool IsAlive => health > 0;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int amt)
    {
        amt = (int)((float)amt * (1f - armorDamageReductionPercent));
        if (health - amt <= 0)
            Die();
        if (IsInvincible)
            return;

        //player has armor
        if(armorPoints > 0)
        {
            armorPoints -= (int)(amt * (1 - armorDamageReductionPercent));

            //damage main health if the armor is now a negative value
            if(armorPoints < 0)
            {
                amt = Math.Abs(armorPoints);

                amt = (int)(amt * (1 - armorDamageReductionPercent));
                health = health - amt > 0 ? health - amt : 0;
            }
        }
        //no armor
        else
        {
            amt = (int)(amt * (1 - armorDamageReductionPercent));

            health = health - amt > 0 ? health - amt : 0;
        }
        
        HealthChanged?.Invoke();
        lastHitTime = Time.time;

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(int Amt)
    {
        health = Mathf.Clamp(health, health + Amt, maxHealth);
        HealthChanged?.Invoke();
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

    public void AddArmor(int amt)
    {
        armorPoints = amt;
        HealthChanged?.Invoke();
    }
}
