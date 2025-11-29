using UnityEngine;

public class NewAmmunitionPickup : MonoBehaviour
{
    public int amount = 20;
    public int weaponIndex = 0;

    public AudioClip pickupSound;
    public float soundVolume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeaponManager w = other.GetComponent<PlayerWeaponManager>();
            if (w != null && w.AddAmmo(amount, weaponIndex))
            {
                if(pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
                }

                Destroy(gameObject);
            }
            else
            {
                PlayerPistol p = other.GetComponent<PlayerPistol>();
                if (p != null)
                {
                    p.ammunition += amount;
                    Destroy(gameObject);
                }
            }
        }
    }
}
