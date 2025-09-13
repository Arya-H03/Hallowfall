using System.Net;
using UnityEngine;

public static class CurveUtils
{
    //end(t) = (1?t)^2 * start + 2(1?t)t * controlPoint + t^2 * end
    public static Vector3 QuadraticBezier(Vector3 start, Vector3 controlPoint, Vector3 end, float t)
    {
        float u = 1 - t;
        return u * u * start + 2 * u * t * controlPoint + t * t * end;
    }

    public static Vector3 ControlPointForMidpoint(Vector3 start, Vector3 end, Vector3 midpoint)
    {
        // 0.5f * 0.5f * start + 2 * 0.5f * 0.5f * controlPoint + 0.5f * 0.5f * end;
        //end(0.5) = 0.25f * start + 0.5 * controlPoint + 0.25f * end

        return 2f * midpoint - 0.5f * (start + end);
    }

    public static float ApproximateLength(Vector3 start, Vector3 control, Vector3 end, int resolution = 20)
    {
        float length = 0f;
        Vector3 prev = start;
        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = QuadraticBezier(start, control, end, t);
            length += Vector3.Distance(prev, point);
            prev = point;
        }
        return length;
    }

    public static Vector3 QuadraticBezierTangent(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        return 2f * (1f - t) * (control - start) + 2f * t * (end - control);
    }

}
