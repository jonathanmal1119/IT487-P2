using UnityEngine;

public class NewAmmunitionPickup : MonoBehaviour
{
    public int amount = 20;
    public int weaponIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWeaponManager w = other.GetComponent<PlayerWeaponManager>();
            if (w != null && w.AddAmmo(amount, weaponIndex))
            {
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
