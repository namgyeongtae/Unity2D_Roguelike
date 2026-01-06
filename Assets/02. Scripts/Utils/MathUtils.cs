using UnityEngine;

public static class MathUtils
{
    public static float EaseInExpo(float t, float k)
    {
        t = Mathf.Clamp01(t);
        if (t <= 0f) return 0f;
        return (Mathf.Exp(k * t) - 1f) / (Mathf.Exp(k) - 1f);
    }

    public static Vector2 AngleToDir(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}
