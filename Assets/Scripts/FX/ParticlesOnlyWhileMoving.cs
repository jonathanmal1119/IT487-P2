using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlesOnlyWhileMoving : MonoBehaviour
{
    Vector3 lastPosition;
    ParticleSystem ps;

    void Start()
    {
        lastPosition = transform.position;
        ps = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if ((transform.position - lastPosition).magnitude < 0.01)
            Invoke(nameof(OnDisable), 0.5f);
        lastPosition = transform.position;
    }

    private void OnDisable()
    {
        ps.Stop();
    }
}
