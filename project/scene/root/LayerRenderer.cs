namespace Tetraizor.Drawing;

using Godot;
using Tetraizor.Data;

public partial class LayerRenderer : Control
{
    public Layer AssignedLayer => _assignedLayer;
    private Layer _assignedLayer;

    public ImageTexture RenderImageTexture { get; private set; }

    public Vector2I CanvasSize => AssignedLayer.CanvasSize;

    private bool _isDirty = false;

    public void Setup(Layer layer, Image image)
    {
        _assignedLayer = layer;

        RenderImageTexture = ImageTexture.CreateFromImage(image);
        if (RenderImageTexture == null)
        {
            GD.PrintErr("LayerRenderer: RenderImageTexture is null.");
        }

        Size = new Vector2(CanvasSize.X, CanvasSize.Y);

        SetDirty();
    }

    public override void _Process(double delta)
    {
        if (_isDirty)
        {
            RenderImageTexture.Update(AssignedLayer.BufferImage);
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
