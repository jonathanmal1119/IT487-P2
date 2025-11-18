using UnityEngine;

[RequireComponent(typeof(Light))]
public class TemporaryLightEffect : MonoBehaviour
{
    public float StartDelay = 0;
    public float Lifetime = 1;
    public AnimationCurve IntensityCurve = AnimationCurve.Linear(0, 1, 1, 0);

    private float startTime;
    private float CurrentTime => Time.time - startTime;
    private float NormalizedTime => Mathf.Clamp(CurrentTime / Lifetime, 0, 1);

    private Light lightComponent;
    private float initialIntensity;


    void Start()
    {
        startTime = Time.time + StartDelay;
        lightComponent = GetComponent<Light>();
        initialIntensity = lightComponent.intensity;
        lightComponent.intensity = 0;
        Destroy(gameObject, StartDelay + Lifetime);
    }

    void Update()
    {
        if (Time.time >= startTime)
            lightComponent.intensity = initialIntensity * IntensityCurve.Evaluate(NormalizedTime);
        else
            lightComponent.intensity = 0;
    }
}
