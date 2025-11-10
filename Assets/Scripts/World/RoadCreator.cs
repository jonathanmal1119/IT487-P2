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

	// Single generated mesh instance to avoid leaking Mesh objects
	private Mesh generatedMesh;

    void OnEnable()
    {
        GenerateRoad();
    }

    void OnValidate()
    {
        GenerateRoad();
    }

	void OnDisable()
	{
		// In play mode, Destroy; in editor, DestroyImmediate to prevent leaks
		if (generatedMesh != null)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) DestroyImmediate(generatedMesh);
			else Destroy(generatedMesh);
#else
			Destroy(generatedMesh);
#endif
			generatedMesh = null;
		}
	}
	
	void OnDestroy()
	{
		// Extra safety: clean up if object is removed
		if (generatedMesh != null)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) DestroyImmediate(generatedMesh);
			else Destroy(generatedMesh);
#else
			Destroy(generatedMesh);
#endif
			generatedMesh = null;
		}
	}

	[ContextMenu("Regenerate Road Mesh")]
    public void GenerateRoad()
    {
        if (splineContainer == null) return;
		if (segments < 2) segments = 2;

		// Create or reuse the generated mesh to prevent leaks
		if (generatedMesh == null)
		{
			generatedMesh = new Mesh();
			generatedMesh.name = "RoadSplineMesh";
			generatedMesh.MarkDynamic();
		}
		else
		{
			generatedMesh.Clear();
		}

        Vector3[] vertices = new Vector3[segments * 2];
        Vector2[] uvs = new Vector2[segments * 2];
        int[] triangles = new int[(segments - 1) * 6];

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

			Vector3 pos = splineContainer.EvaluatePosition(t);
			Vector3 tangent = splineContainer.EvaluateTangent(t);
            tangent = tangent.normalized;

			// Simple frame: cross with world up; basic fallback if degenerate
			Vector3 right = Vector3.Cross(Vector3.up, tangent);
			if (right.sqrMagnitude < 1e-8f) right = Vector3.right;
			right = right.normalized;

			Vector3 leftWorld = pos - right * (roadWidth * 0.5f);
			Vector3 rightWorld = pos + right * (roadWidth * 0.5f);
			vertices[i * 2] = transform.InverseTransformPoint(leftWorld);
			vertices[i * 2 + 1] = transform.InverseTransformPoint(rightWorld);

			// Simple UVs: v goes 0..1 along the spline
			uvs[i * 2] = new Vector2(0f, t);
			uvs[i * 2 + 1] = new Vector2(1f, t);

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

        generatedMesh.vertices = vertices;
        generatedMesh.uv = uvs;
        generatedMesh.triangles = triangles;
        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateBounds();

        var mf = GetComponent<MeshFilter>();
		if (mf != null)
		{
			mf.sharedMesh = generatedMesh;
		}

		// Update MeshCollider if present
		var meshCollider = GetComponent<MeshCollider>();
		if (meshCollider != null)
		{
			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = generatedMesh;
		}
    }
}
