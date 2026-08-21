using System;
using UnityEngine;

public static class MathExtensions
{
    public static int GetTaxicabDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public static float Frac(float f)
    {
        return (float)(f - Math.Truncate(f));
    }
}