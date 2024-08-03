using Godot;

public static class Vector2Utils
{
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(
            Mathf.Lerp(a.X, b.X, t),
            Mathf.Lerp(a.Y, b.Y, t)
        );
    }
}