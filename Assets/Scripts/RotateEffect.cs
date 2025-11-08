using UnityEngine;

public class RotateEffect : MonoBehaviour
{
    public Vector3 rotationRates;

    void Update()
    {
        transform.Rotate(rotationRates * Time.deltaTime);
    }
}
