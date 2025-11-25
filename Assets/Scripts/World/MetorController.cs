using UnityEngine;

public class MoveParentForward : MonoBehaviour
{
    public float speed = 5f;
    public float speedSlider = 5f;

    void Update()
    {
        speed = speedSlider;

        transform.Translate(-Vector3.right * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1000);
        }
    }
}
