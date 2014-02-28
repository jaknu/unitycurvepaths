using UnityEngine;
using System.Collections;

[System.Serializable]
public class PointWithTangent
{
    public Vector3 point;
    public Vector3 tangent;
}

public class FollowHermiteCurve : MonoBehaviour 
{
    static Color GIZMO_COLOR_HELPERS = Color.grey;
    static Color GIZMO_COLOR_CURVE = Color.white;
    const float GIZMO_SPHERE_SIZE = 0.1f;
    const int GIZMO_CURVE_LINE_SEGMENTS = 100;

    public PointWithTangent[] points;
    public float duration;

    float _startTime;
    Transform _transform;

    public void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    public void Start()
    {
        _startTime = Time.time;
    }

    public void Update()
    {
        _transform.position = position(Mathf.Clamp01((Time.time - _startTime) / duration));
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = GIZMO_COLOR_HELPERS;

        if (points.Length < 1)
            return;
        
        for (int i = 0; i < points.Length; i++) {
            Gizmos.DrawSphere(points[i].point, GIZMO_SPHERE_SIZE);
            Gizmos.DrawLine(points[i].point, points[i].point + points[i].tangent);
        }

        Vector3 p = points[0].point;
        for (int i = 1; i < points.Length; i++) {
            Gizmos.DrawLine(p, points[i].point);
            p = points[i].point;
        }

        Gizmos.color = GIZMO_COLOR_CURVE;
        p = points[0].point;
        for (float i = 0f; i <= (float)GIZMO_CURVE_LINE_SEGMENTS; i += 1f) {
            Vector3 pNew = position(Mathf.Clamp01(i / (float)GIZMO_CURVE_LINE_SEGMENTS));
            Gizmos.DrawLine(p, pNew);
            p = pNew;
        }
    }

    Vector3 position(float t)
    {
        int segmentCount = points.Length - 1;

        float segmentLength = 1f / segmentCount;
        int segmentIndex = Mathf.Clamp(Mathf.FloorToInt(t / segmentLength), 0, segmentCount-1);

        float segmentT = (t - segmentIndex * segmentLength) / segmentLength;

        return hermite(segmentT, points[segmentIndex], points[segmentIndex+1]);
    }

    static Vector3 hermite(float t, PointWithTangent a, PointWithTangent b)
    {
        return (2f*t*t*t - 3f*t*t + 1f) * a.point
             + (-2f*t*t*t + 3f*t*t) * b.point
             + (t*t*t - 2f*t*t + t) * a.tangent
             + (t*t*t - t*t) * b.tangent;
    }
}
