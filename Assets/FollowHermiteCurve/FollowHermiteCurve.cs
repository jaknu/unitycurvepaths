using UnityEngine;
using System.Collections;

[System.Serializable]
public class PointWithTangent
{
    public Vector3 point;
    public Vector3 tangent;
}

/*
 * This script is intended as an example or starting point for having an object follow a hermite curve.
 * The gameobject this is attached to will follow a path which takes it through each of the given points
 * following the direction of the associated tangent at each one.
 * 
 * The hermite curve formula is yanked from from section 10.2.3 of the excellent book 
 * "Essential Mathematics For Games & Interactive Applications, A Programmers Guide, Second Edition",
 * by James M. Van Verth and Lars M. Bishop. (ISBN: 978-0-12-374297-1)
 */
public class FollowHermiteCurve : MonoBehaviour 
{
    static Color GIZMO_COLOR_HELPERS       = Color.grey;
    static Color GIZMO_COLOR_CURVE         = Color.white;
    const float  GIZMO_SPHERE_SIZE         = 0.1f;
    const int    GIZMO_CURVE_LINE_SEGMENTS = 100;

    public PointWithTangent[] points;
    public float duration;
    public bool loop;

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
        float t = (Time.time - _startTime) / duration;
        if (loop) {
            t = t % 1f;
        } else {
            t = Mathf.Clamp01(t);
        }

        _transform.position = position(t);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = GIZMO_COLOR_HELPERS;

        if (points.Length < 1) {
            return;
        }

        for (int i = 0; i < points.Length; i++) {
            Gizmos.DrawSphere(points[i].point, GIZMO_SPHERE_SIZE);
            Gizmos.DrawLine(points[i].point, points[i].point + points[i].tangent);
        }

        Vector3 p = points[0].point;
        for (int i = 1; i < points.Length + (loop ? 1 : 0); i++) {
            Gizmos.DrawLine(p, points[i % points.Length].point);
            p = points[i % points.Length].point;
        }

        Gizmos.color = GIZMO_COLOR_CURVE;
        p = points[0].point;
        for (float i = 0f; i <= (float)GIZMO_CURVE_LINE_SEGMENTS; i += 1f) {
            Vector3 pNew = position(Mathf.Clamp01(i / (float)GIZMO_CURVE_LINE_SEGMENTS));
            Gizmos.DrawLine(p, pNew);
            p = pNew;
        }
    }

    /*
     * Get the point on the path for time t where t <= 0.0 will yield the position of
     * the first point on the path and t >= 1.0 will yield the position of the last.
     */
    Vector3 position(float t)
    {
        int segmentCount = points.Length - (loop ? 0 : 1);
        float segmentLength = 1f / segmentCount;
        int segmentIndex = Mathf.Clamp(
            Mathf.FloorToInt(t / segmentLength), 
            0, 
            segmentCount-1
        );

        float segmentT = (t - segmentIndex * segmentLength) / segmentLength;

        return hermite(
            segmentT, 
            points[segmentIndex], 
            points[(segmentIndex+1) % points.Length]
        );
    }

    /* 
     * Get the point on the path between points a and b for the "time" t where
     * t <= 0.0 will yield the position a and t >= will yield b.
     * 
     * Think of it as a hermite interpolation.
     */
    static Vector3 hermite(float t, PointWithTangent a, PointWithTangent b)
    {
        return (2f*t*t*t - 3f*t*t + 1f) * a.point
             + (-2f*t*t*t + 3f*t*t) * b.point
             + (t*t*t - 2f*t*t + t) * a.tangent
             + (t*t*t - t*t) * b.tangent;
    }
}
