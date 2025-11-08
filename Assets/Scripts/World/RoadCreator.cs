using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadSplineGenerator : MonoBehaviour
{
    public SplineContainer splineContainer;
    [Range(0.5f, 50f)] public float roadWidth = 2f;
    [Range(4, 500)] public int segments = 100;
	[Min(0.01f)] public float metersPerTile = 1f;

	// Returns true if all components are finite numbers
	private static bool IsFinite(Vector3 v)
	{
		return !(float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z) ||
			float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z));
	}

    void OnEnable()
    {
        GenerateRoad();
    }

    void OnValidate()
    {
        GenerateRoad();
    }

	void Update()
	{
		if (!Application.isPlaying)
		{
			GenerateRoad();
		}
	}

	[ContextMenu("Regenerate Road Mesh")]
    public void GenerateRoad()
    {
        if (splineContainer == null) return;
		if (segments < 2) segments = 2;

		// Early validation: if spline evaluates to invalid values, skip to avoid invalid meshes
		Vector3 p0 = splineContainer.EvaluatePosition(0f);
		Vector3 p1 = splineContainer.EvaluatePosition(1f);
		Vector3 tMid = splineContainer.EvaluateTangent(0.5f);
		if (!IsFinite(p0) || !IsFinite(p1) || !IsFinite(tMid))
		{
			return;
		}

        Mesh mesh = new Mesh();
		mesh.name = "RoadSplineMesh";
        Vector3[] vertices = new Vector3[segments * 2];
        Vector2[] uvs = new Vector2[segments * 2];
        int[] triangles = new int[(segments - 1) * 6];

		float vCoord = 0f;
		Vector3 prevPos = Vector3.zero;
		bool hasPrev = false;
		bool invalid = false;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

			// Evaluate along the spline (positions and tangents are in world space for SplineContainer)
			Vector3 pos = splineContainer.EvaluatePosition(t);
			Vector3 tangent = splineContainer.EvaluateTangent(t);
			if (!IsFinite(pos) || !IsFinite(tangent))
			{
				invalid = true;
				break;
			}
            tangent = tangent.normalized;

			// Choose an up-axis that avoids degeneracy when tangent is near world up
			Vector3 upAxis = Mathf.Abs(Vector3.Dot(tangent, Vector3.up)) > 0.99f ? Vector3.right : Vector3.up;
            Vector3 right = Vector3.Cross(upAxis, tangent);
			if (right.sqrMagnitude < 1e-8f)
			{
				right = Vector3.right; // final fallback
			}
			right = right.normalized;

			if (hasPrev)
			{
				float meters = Vector3.Distance(prevPos, pos);
				vCoord += meters / Mathf.Max(0.01f, metersPerTile);
			}
			prevPos = pos;
			hasPrev = true;

			// Convert to this MeshFilter's local space so the mesh sits correctly under this object
			Vector3 leftWorld = pos - right * (roadWidth * 0.5f);
			Vector3 rightWorld = pos + right * (roadWidth * 0.5f);
			vertices[i * 2] = transform.InverseTransformPoint(leftWorld);
			vertices[i * 2 + 1] = transform.InverseTransformPoint(rightWorld);

            uvs[i * 2] = new Vector2(0, vCoord);
            uvs[i * 2 + 1] = new Vector2(1, vCoord);

            if (i < segments - 1)
            {
                int tri = i * 6;
                int vi = i * 2;
                triangles[tri] = vi;
                triangles[tri + 1] = vi + 2;
                triangles[tri + 2] = vi + 1;
                triangles[tri + 3] = vi + 1;
                triangles[tri + 4] = vi + 2;
                triangles[tri + 5] = vi + 3;
            }
        }

		if (invalid)
		{
			return;
		}

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;

		// Update MeshCollider if present
		var meshCollider = GetComponent<MeshCollider>();
		if (meshCollider != null)
		{
			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = mesh;
		}

#if UNITY_EDITOR
        if (!Application.isPlaying)
		{
            EditorUtility.SetDirty(this);
			var mf = GetComponent<MeshFilter>();
			if (mf != null) EditorUtility.SetDirty(mf);
			if (meshCollider != null) EditorUtility.SetDirty(meshCollider);
		}
#endif
    }
}
