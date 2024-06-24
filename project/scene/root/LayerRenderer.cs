namespace Tetraizor.Drawing;

using Godot;
using System;
using Tetraizor.Managers;

public partial class LayerRenderer : Node2D
{
    public Image RenderImage { get; private set; }
    public Image BufferImage { get; private set; }
    public Canvas ParentCanvas { get; private set; }

    public ImageTexture RenderImageTexture { get; private set; }

    public Vector2I Size => ParentCanvas.Size;

    private bool _isDirty = false;

    public void Setup(Canvas parentCanvas, Image image)
    {
        ParentCanvas = parentCanvas;
        RenderImage = image;

        // Copy the image data to the buffer image.
        BufferImage = new Image();
        BufferImage.CopyFrom(image);

        RenderImageTexture = ImageTexture.CreateFromImage(image);
    }

    public override void _Process(double delta)
    {
        if (_isDirty)
        {
            RenderImageTexture.Update(BufferImage);
            QueueRedraw();
            _isDirty = false;
        }
    }

    public override void _Draw()
    {
        DrawTexture(RenderImageTexture, new Vector2(0, 0));
    }

    public void SetDirty()
    {
        _isDirty = true;
    }
}
