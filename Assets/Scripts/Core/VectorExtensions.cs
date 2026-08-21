using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 WithY(this Vector2 v, float y)
    {
        return new Vector3(v.x, y, v.y);
    }

    public static Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithY(this Vector2Int v, float y)
    {
        return new Vector3(v.x, y, v.y);
    }

    public static Vector3 WithZ(this Vector2Int v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 WithoutY(this Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }

    public static Vector3 ToXZ(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    public static Vector2 ToXZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector2Int ToXZInt(this Vector3 v)
    {
        return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.z));
    }

    public static float Dot(Vector2Int a, Vector2Int b)
    {
        return (a.x * b.x) + (a.y * b.y);
    }
}