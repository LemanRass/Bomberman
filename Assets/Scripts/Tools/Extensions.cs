using UnityEngine;

public static class Extensions
{
    public static Vector2 To2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 To3(this Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }
}
