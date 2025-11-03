using UnityEngine;

public class AmmunitionPickup : MonoBehaviour
{
    public int amount = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPistol p = other.GetComponent<PlayerPistol>();
            if(p != null)
            {
                p.ammunition += amount;
                Destroy(gameObject);
            }
        }
    }
}
