namespace Tetraizor.Drawing;

public class Brush
{
    private float _size;
    public float Size => _size;

    public Brush(float size)
    {
        _size = size;
    }
}