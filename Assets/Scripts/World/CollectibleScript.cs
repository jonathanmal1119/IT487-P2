using UnityEngine;

public class CollectibleScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>().ShowExplosiveHUD();
            other.GetComponent<PlayerObjectiveData>().GiveExplosive();
        }
        Destroy(gameObject);
    }
}
