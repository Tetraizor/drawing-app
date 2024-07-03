using System.Collections.Generic;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Drawing;

namespace Tetraizor.Managers;

public partial class CanvasManager : AutoloadBase<CanvasManager>
{
    private Texture _originalTexture;
    private Texture _bufferTexture;

    private List<Canvas> _canvasList = new();
    public List<Canvas> CanvasList => _canvasList;

    // TODO: Remove this method.
    public override void _Ready()
    {
        base._Ready();

        CreateEmptyCanvas(new Vector2I(2048, 2048));
    }

    public Canvas GetCanvas(int index)
    {
        if (index < 0 || index >= _canvasList.Count) throw new System.IndexOutOfRangeException();

        return _canvasList[index];
    }

    public LayerRenderer GetLayer(Canvas canvas, int index)
    {
        if (canvas == null) throw new System.ArgumentNullException(nameof(canvas));
        if (index < 0 || index >= canvas.Layers.Count) throw new System.IndexOutOfRangeException();

        return canvas.Layers[index];
    }

    public void CreateEmptyCanvas(Vector2I size)
    {
        Control layerContainer = new Control();
        layerContainer.MouseFilter = Control.MouseFilterEnum.Ignore;
        AddChild(layerContainer);

        var canvas = new Canvas(layerContainer, size);

        layerContainer.Size = new Vector2(size.X, size.Y);
        layerContainer.Position = new Vector2(0, 0);

        var background = new Image();
        background.SetData(size.X, size.Y, false, Image.Format.Rgba8, new byte[size.X * size.Y * 4]);
        background.Fill(Colors.White);

        var layer = new Image();
        layer.SetData(size.X, size.Y, false, Image.Format.Rgba8, new byte[size.X * size.Y * 4]);
        layer.Fill(Colors.Transparent);

        CreateLayer(canvas, background);
        CreateLayer(canvas, layer);

        _canvasList.Add(canvas);

        layerContainer.Name = $"Canvas{_canvasList.Count - 1}";
    }

    public LayerRenderer CreateLayer(Canvas canvas, Image image)
    {
        var layer = new LayerRenderer();
        layer.Setup(canvas, image);
        layer.Name = $"Layer{canvas.Layers.Count}";

        canvas.LayerContainer.AddChild(layer);

        canvas.Layers.Add(layer);

        return layer;
    }
}

public class Canvas
{
    public Vector2I Size { get; private set; }
    public List<LayerRenderer> Layers { get; private set; } = new();
    public Control LayerContainer { get; private set; }

    public Canvas(Control layerContainer, Vector2I size)
    {
        Size = size;
        LayerContainer = layerContainer;
    }
}