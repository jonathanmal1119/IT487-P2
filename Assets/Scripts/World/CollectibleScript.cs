using UnityEngine;

public class CollectibleScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerObjectiveData>().UnlockWheel();
        }

        Destroy(gameObject);
    }
}
