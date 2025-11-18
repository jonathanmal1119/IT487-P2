using UnityEngine;

public class PlayerObjectiveData : MonoBehaviour
{
    public bool hasWheel = false;


    public void UnlockWheel()
    {
        hasWheel = true;
    }
}
