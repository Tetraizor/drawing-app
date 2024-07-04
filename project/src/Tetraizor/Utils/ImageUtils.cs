using Godot;

namespace Tetraizor.Utils;

public static class ImageUtils
{
    public static void Create(this Image image, int width, int height, Color color)
    {
        image.SetData(width, height, false, Image.Format.Rgba8, new byte[width * height * 4]);
        image.Fill(color);
    }

    public static void Create(this Image image, int width, int height)
    {
        image.Create(width, height, Colors.White);
    }

    public static void Create(this Image image, Vector2I size, Color color)
    {
        image.Create(size.X, size.Y, color);
    }

    public static void Create(this Image image, Vector2I size)
    {
        image.Create(size, Colors.White);
    }
}