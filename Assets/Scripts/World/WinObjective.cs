using UnityEngine;

public class WinObjective : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>().ShowWinScreen();
    }
}
