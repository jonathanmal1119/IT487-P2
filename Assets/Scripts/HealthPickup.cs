using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int amount = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth w = other.GetComponent<PlayerHealth>();
            if (w != null && w.health < w.maxHealth)
            {
                w.Heal(amount);
                Destroy(gameObject);
            }
        }
    }
}
