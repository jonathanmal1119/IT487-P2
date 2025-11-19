using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshRenderDistance : MonoBehaviour
{
    public float RenderMinDistance = 10;

    public AnimationCurve ScaleCurve = AnimationCurve.Linear(0, 1, 1, 1);

    public float CurveStart = 1;
    public float CurveEnd = 1;

    Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        if ((Camera.main.transform.position - transform.position).magnitude > RenderMinDistance)
        {
            GetComponent<MeshRenderer>().enabled = true;

            transform.localScale = startScale * ScaleCurve.Evaluate(((Camera.main.transform.position - transform.position).magnitude - CurveStart) / (CurveEnd - CurveStart));
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
