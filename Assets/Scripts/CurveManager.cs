using UnityEngine;
using System.Collections.Generic;

public enum CurveType
{
    QuadraticBezier, // 二次贝塞尔 3个点
    CubicBezier      // 三次贝塞尔 4个点
}

public class CurveManager : MonoBehaviour
{
    [Header("设置")]
    public CurveType curveType;
    public GameObject controlPointPrefab;
    public int curveSampleCount = 100;
    public LineRenderer lineRenderer;

    public List<Transform> controlPoints = new List<Transform>();

    void Start()
    {
        SpawnControlPoints();
        UpdateCurve();
    }

    void SpawnControlPoints()
    {
        foreach (var t in controlPoints) Destroy(t.gameObject);
        controlPoints.Clear();

        int pointCount = curveType == CurveType.QuadraticBezier ? 3 : 4;
        Vector3 startPos = new Vector3(-4, 0, 0);
        float offset = 2.5f;

        for (int i = 0; i < pointCount; i++)
        {
            GameObject pt = Instantiate(controlPointPrefab, startPos + Vector3.right * offset * i, Quaternion.identity);
            ControlPointDrag drag = pt.GetComponent<ControlPointDrag>();
            // 这一行是修复点！赋值给 curveManagerObj，不再用 curveManager
            drag.curveManagerObj = this.gameObject;
            controlPoints.Add(pt.transform);
        }
    }

    public void UpdateCurve()
    {
        if (lineRenderer == null) return;
        lineRenderer.positionCount = curveSampleCount;

        for (int i = 0; i < curveSampleCount; i++)
        {
            float t = (float)i / (curveSampleCount - 1);
            Vector3 pos = ComputeCurvePoint(t);
            lineRenderer.SetPosition(i, pos);
        }
    }

    Vector3 ComputeCurvePoint(float t)
    {
        if (curveType == CurveType.QuadraticBezier)
        {
            Vector3 p0 = controlPoints[0].position;
            Vector3 p1 = controlPoints[1].position;
            Vector3 p2 = controlPoints[2].position;
            return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
        }
        else
        {
            Vector3 p0 = controlPoints[0].position;
            Vector3 p1 = controlPoints[1].position;
            Vector3 p2 = controlPoints[2].position;
            Vector3 p3 = controlPoints[3].position;
            return Mathf.Pow(1 - t, 3) * p0
                + 3 * Mathf.Pow(1 - t, 2) * t * p1
                + 3 * (1 - t) * t * t * p2
                + t * t * t * p3;
        }
    }

    public List<Vector3> GetCurveSamplePoints()
    {
        List<Vector3> pts = new List<Vector3>();
        for (int i = 0; i < curveSampleCount; i++)
        {
            float t = (float)i / (curveSampleCount - 1);
            pts.Add(ComputeCurvePoint(t));
        }
        return pts;
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            SpawnControlPoints();
            UpdateCurve();
        }
    }
}
