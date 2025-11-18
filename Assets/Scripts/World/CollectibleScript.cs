using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    //Temp
    GameObject WinScreen;

    private void Start()
    {
        WinScreen = GameObject.FindGameObjectWithTag("UI").transform.Find("Win Screen").gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerObjectiveData>().UnlockWheel();
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLookControls>().enabled = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWalkControls>().enabled = false;
        WinScreen.SetActive(true);
        Destroy(gameObject);
    }
}
