using Godot;

namespace Tetraizor.Drawing;

public class Stroke
{
    private Vector2I _position;
    public Vector2I Position => _position;

    private float _pressure;
    public float Pressure => _pressure;

    public Stroke(Vector2I position, float sensitivity = 1)
    {
        _position = position;
        _pressure = sensitivity;
    }
}