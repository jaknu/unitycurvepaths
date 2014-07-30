using UnityEngine;
using System.Collections;

public class FollowBezierCurve : MonoBehaviour 
{
    static Color GIZMO_COLOR_HELPERS       = Color.grey;
    static Color GIZMO_COLOR_CURVE         = Color.white;
    const float  GIZMO_SPHERE_SIZE         = 0.1f;
    const int    GIZMO_CURVE_LINE_SEGMENTS = 100;

    public Vector3[] points;
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
        _transform.position = bezier(
            Mathf.Clamp01((Time.time - _startTime) / duration), 
            points
        );
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = GIZMO_COLOR_HELPERS;
        foreach (Vector3 point in points) {
            Gizmos.DrawSphere(point, GIZMO_SPHERE_SIZE);
        }

        if (points.Length < 1) {
            return;
        }

        Vector3 p = points[0];
        for (int i = 1; i < points.Length; i++) {
            Gizmos.DrawLine(p, points[i]);
            p = points[i];
        }

        Gizmos.color = GIZMO_COLOR_CURVE;
        p = points[0];
        for (float i = 0f; i <= (float)GIZMO_CURVE_LINE_SEGMENTS; i += 1f) {
            Vector3 pNew = bezier(
                Mathf.Clamp01(i / (float)GIZMO_CURVE_LINE_SEGMENTS), 
                points
            );
            Gizmos.DrawLine(p, pNew);
            p = pNew;
        }
    }

    static Vector3 bezier(float t, params Vector3[] points)
    {
        if (points.Length < 1) {
            throw new System.ArgumentException("A bezier curve cannot be defined with no points.");
        }

        uint n = (uint)points.Length - 1;
        Vector3 sum = Vector3.zero;
        for (uint i = 0; i <= n; i++) {
            sum += bernsteinBasisPolynomial(i, n, t) * points[i];
        }

        return sum;
    }
    
    static float bernsteinBasisPolynomial(uint i, uint n, float t)
    {
        return binomial_coefficient(n, i) * Mathf.Pow(t, i) * Mathf.Pow(1f - t, n - i);
    }

    static float binomial_coefficient(uint n, uint i)
    {
        return (float)factorial(n) / (float)(factorial(i) * factorial(n - i));
    }

    static uint factorial(uint n)
    {
        if (n <= 1) {
            return 1;
        }

        uint result = n;
        for (uint i = n; i > 1; i--) {
            result *= i;
        }

        return result;
    }
}
