namespace Tetraizor.Data;

using System;
using Godot;
using Tetraizor.Drawing;
using Tetraizor.Managers;
using Tetraizor.Utils;

public partial class LayerData : IDisposable
{
    /// <summary>
    /// The image that has the latest changes on it. Doesn't actually render to the screen.
    /// Stores the latest manipulated version of the image.<br/>
    /// When changes to buffer image are made, the render image is updated to be a copy of it.
    /// </summary>
    public Image RenderImage => _renderImage;
    private Image _renderImage = new();

    /// <summary>
    /// The image data that is rendered to the screen with the help of <b>RenderImageTexture</b>, 
    /// manipulations will be seen on this image.
    /// Use <see cref="LayerRenderer.SetDirty">SetDirty</see> method to update the image data to be actually rendered.
    /// </summary>
    public Image BufferImage => _bufferImage;
    private Image _bufferImage = new();

    public string DisplayName => _displayName;
    private string _displayName;

    public LayerRenderer Renderer => _renderer;
    private LayerRenderer _renderer;

    public CanvasManager CanvasManager => _canvasManager;
    private CanvasManager _canvasManager;

    // Indirect References
    public ImageTexture RendererImageTexture => _renderer.RenderImageTexture;
    public int Index => _canvasManager.Layers.IndexOf(this);
    public Vector2I CanvasSize => _canvasManager.Size;

    public delegate void LayerPropertyChangedEventHandler(LayerData layerData);
    public event LayerPropertyChangedEventHandler LayerPropertyChanged = delegate { };

    public LayerData(CanvasManager canvasManager, Color clearColor)
    {
        Setup(canvasManager, clearColor);
    }

    public LayerData(CanvasManager canvasManager)
    {
        Setup(canvasManager, Colors.White);
    }

    public LayerData(CanvasManager canvasManager, Image image)
    {
        Setup(canvasManager, Colors.White);
        PasteAt(image);
    }

    private void Setup(CanvasManager canvasManager, Color clearColor)
    {
        _canvasManager = canvasManager;
        _displayName = $"Layer {_canvasManager.Layers.Count + 1}";

        _canvasManager.Layers.Add(this);

        _renderImage.Create(CanvasSize, clearColor);
        _bufferImage.CopyFrom(_renderImage);

        _renderer = CanvasManager.Instance.CreateLayerRenderer();
        _renderer.Name = _displayName;

        _canvasManager.LayerContainer.AddChild(_renderer);

        _renderer.Setup(this, _renderImage);
    }

    // Delete layer
    public void Delete()
    {
        throw new NotImplementedException();
    }

    // Move layer
    public void Move(int index)
    {
        throw new NotImplementedException();
    }

    public void PasteAt(Image image, Vector2I offset = default)
    {
        if (image.GetSize() != CanvasSize) throw new ArgumentException("Image size must be the same as the layer size.");

        _bufferImage.BlitRect(image, new Rect2I(Vector2I.Zero, image.GetSize()), offset);
    }

    public void Dispose()
    {
        _renderImage.Dispose();
        _renderer.QueueFree();
    }

    public void ChangeName(string name)
    {
        _displayName = name;
        _renderer.Name = name;

        LayerPropertyChanged?.Invoke(this);
    }
}