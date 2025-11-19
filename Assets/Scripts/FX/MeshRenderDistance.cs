using Assets.Scripts;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshRenderDistance : MonoBehaviour
{
    public float RenderMinDistance = 10;

    public AnimationCurve ScaleCurve = AnimationCurve.Linear(0, 1, 1, 1);

    public float CurveStart = 1;
    public float CurveEnd = 1;

    public float MinSpeed = 0.1f;

    Vector3 startScale;

    Vector3 lastPos;

    void Start()
    {
        startScale = transform.localScale;
        lastPos = transform.position;
    }

    void Update()
    {
        if ((Utils.CurrentCamera.transform.position - transform.position).magnitude > RenderMinDistance && (transform.position - lastPos).magnitude > MinSpeed)
        {
            GetComponent<MeshRenderer>().enabled = true;

            transform.localScale = startScale * ScaleCurve.Evaluate(((Utils.CurrentCamera.transform.position - transform.position).magnitude - CurveStart) / (CurveEnd - CurveStart));
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        lastPos = transform.position;
    }
}
