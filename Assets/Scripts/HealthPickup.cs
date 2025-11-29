using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int amount = 20;

    public AudioClip pickupSound;
    public float soundVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth w = other.GetComponent<PlayerHealth>();
            if (w != null && w.health < w.maxHealth)
            {
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
                }

                w.Heal(amount);
                Destroy(gameObject);
            }
        }
    }
}
