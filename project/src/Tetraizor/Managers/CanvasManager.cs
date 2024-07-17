using System.Collections.Generic;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Data;
using Tetraizor.Drawing;

namespace Tetraizor.Managers;

public partial class CanvasManager : AutoloadBase<CanvasManager>
{
    #region Canvas Properties
    [Export] private PackedScene _layerRendererScene = GD.Load<PackedScene>("res://prefab/ui/layer_renderer.tscn");

    public Vector2I Size => _size;
    private Vector2I _size;

    public List<LayerData> Layers => _layers;
    private List<LayerData> _layers = new();

    public LayerData CurrentLayer => _currentLayer;
    private LayerData _currentLayer;
    #endregion

    #region Signals
    [Signal] public delegate void LayerCreatedEventHandler(int layerIndex);
    [Signal] public delegate void LayerDeletedEventHandler(int layerIndex);
    [Signal] public delegate void LayerSelectedEventHandler(int layerIndex);
    #endregion

    #region References
    public Control LayerContainer => _layerContainer;
    private Control _layerContainer;
    #endregion

    #region Godot Methods
    // TODO: Remove this method.
    public override void _Ready()
    {
        base._Ready();
        CreateCanvas(new Vector2I(512, 512));
    }
    #endregion

    #region Canvas Methods

    public void CreateCanvas(Vector2I size)
    {
        _size = size;

        _layerContainer = new Control
        {
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Name = "LayerContainer",
            Position = new Vector2(0, 0),
            Size = new Vector2(size.X, size.Y),
        };

        AddChild(_layerContainer);

        var clearRenderer = new ColorRect
        {
            Color = Colors.White,
            Size = size
        };

        _layerContainer.AddChild(clearRenderer);

        CreateLayer();
    }
    #endregion

    #region Layer Methods

    public LayerRenderer CreateLayerRenderer()
    {
        var layerRenderer = _layerRendererScene.Instantiate<LayerRenderer>();
        return layerRenderer;
    }

    public LayerData CreateLayer(Image image = null)
    {
        var newLayer = image == null ? new LayerData(this, Colors.Transparent) : new LayerData(this, image);

        _currentLayer = newLayer;

        EmitSignal(SignalName.LayerCreated, Layers.IndexOf(newLayer));

        SelectLayer(newLayer);

        return newLayer;
    }

    public void DeleteLayer(int layerIndex)
    {
        if (layerIndex < 0 || layerIndex >= Layers.Count) throw new System.IndexOutOfRangeException();

        DeleteLayer(Layers[layerIndex]);
    }

    public void DeleteLayer(LayerData layer)
    {
        if (layer == null) throw new System.ArgumentNullException(nameof(layer));
        if (!_layers.Contains(layer)) throw new System.ArgumentException("Layer not found.");

        _layers.Remove(layer);
        layer.Dispose();

        EmitSignal(SignalName.LayerDeleted, Layers.IndexOf(layer));
    }

    public void SelectLayer(LayerData layer)
    {
        if (layer == null) throw new System.ArgumentNullException(nameof(layer));
        if (!_layers.Contains(layer)) throw new System.ArgumentException("Layer not found.");

        SelectLayer(Layers.IndexOf(layer));
    }

    public void SelectLayer(int index)
    {
        if (index < 0 || index >= Layers.Count) throw new System.IndexOutOfRangeException();

        _currentLayer = Layers[index];
        EmitSignal(SignalName.LayerSelected, index);
    }

    public void MoveLayer(int layerIndex, int newIndex)
    {
        if (layerIndex == newIndex) return;
        if (layerIndex < 0 || layerIndex >= Layers.Count) throw new System.IndexOutOfRangeException();
        if (newIndex < 0 || newIndex >= Layers.Count) throw new System.IndexOutOfRangeException();

        LayerContainer.MoveChild(Layers[layerIndex].Renderer, newIndex + 1);

        var layer = Layers[layerIndex];
        Layers.RemoveAt(layerIndex);
        Layers.Insert(newIndex, layer);
    }
    #endregion
}