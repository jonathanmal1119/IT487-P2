using UnityEngine;

public class PlayerObjectiveData : MonoBehaviour
{
    [SerializeField]
    public bool hasExplosive = false;

    public void GiveExplosive()
    {
        hasExplosive = true;
    }
}
