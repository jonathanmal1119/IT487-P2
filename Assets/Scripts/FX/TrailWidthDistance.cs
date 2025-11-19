using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailWidthDistance : MonoBehaviour
{
    public AnimationCurve WidthCurve = AnimationCurve.Linear(0, 1, 1, 1);

    public float CurveStart = 1;
    public float CurveEnd = 1;

    TrailRenderer trailRenderer;

    float startWidth;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        startWidth = trailRenderer.widthMultiplier;
    }

    void Update()
    {
        trailRenderer.widthMultiplier = startWidth * WidthCurve.Evaluate(((Camera.main.transform.position - transform.position).magnitude - CurveStart) / (CurveEnd - CurveStart));
    }
}
