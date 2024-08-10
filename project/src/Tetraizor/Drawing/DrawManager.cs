using System;
using System.Diagnostics;
using Godot;
using Tetraizor.Autoloads.ActionMemoryManagement;
using Tetraizor.Data;
using Tetraizor.Managers;
using Tetraizor.Utils;

namespace Tetraizor.Drawing;

public partial class DrawManager : Node
{
    public enum DrawMode { Override, Keep }
    private CanvasManager _canvasManager;

    private LayerData _currentLayer;

    private ImageTexture _renderTexture;

    private Image _bufferImage;

    private bool _isRectDirty = false;
    private Rect2I _dirtyRect;

    public void SelectLayer(int layerIndex)
    {
        _currentLayer = _canvasManager.Layers[layerIndex];

        _renderTexture = _currentLayer.Renderer.RenderImageTexture;
        _bufferImage = _currentLayer.BufferImage;
    }

    #region Godot Methods
    public override void _Ready()
    {
        _canvasManager = CanvasManager.Instance;

        if (_canvasManager == null)
        {
            GD.PrintErr("DrawManager: CanvasManager is null.");
        }

        CanvasManager.Instance.LayerCreated += OnLayerCreated;
        CanvasManager.Instance.LayerDeleted += OnLayerDeleted;
        CanvasManager.Instance.LayerSelected += OnLayerSelected;

        SelectLayer(0);
    }

    private void OnLayerSelected(int layerIndex)
    {
        SelectLayer(layerIndex);
    }

    private void OnLayerDeleted(int layerIndex)
    {
    }

    private void OnLayerCreated(int layerIndex)
    {
    }

    public override void _Process(double delta)
    {
        // TODO: Change this with it's partial counterpart when Godot 4.3 comes.
        // TODO: Feature is not implemented yet, and is very performance heavy currently.
        if (_isRectDirty)
        {
            _currentLayer.Renderer.SetDirty();
            _isRectDirty = false;
        }
    }
    #endregion

    /// <summary>
    /// Copy buffer to render image.
    /// </summary>
    public void SaveBuffer()
    {
        var bufferImage = _currentLayer.BufferImage;
        _currentLayer.RenderImage.CopyFrom(bufferImage);
    }

    public void FinishStroke()
    {
        var previousImage = _currentLayer.RenderImage.Duplicate() as Image;
        var currentImage = _currentLayer.BufferImage.Duplicate() as Image;

        SaveBuffer();
        ClearBuffer();
        ActionMemoryManager.AddAction(new DrawAction(_currentLayer, previousImage, currentImage));
    }

    /// <summary>
    /// Copy render to buffer image.
    /// </summary>
    public void ClearBuffer()
    {
        _bufferImage.CopyFrom(_currentLayer.RenderImage);
    }

    public void DrawTextureToBuffer(Image image, Vector2I position, DrawMode drawMode, ColorUtils.BlendMode blendMode)
    {
        if (drawMode == DrawMode.Override)
            _bufferImage.CopyFrom(_currentLayer.RenderImage);

        BlendImage(_bufferImage, image, position, blendMode);

        _isRectDirty = true;
    }

    public void BlendImage(Image backgroundImage, Image overlayImage, Vector2I position, ColorUtils.BlendMode blendMode)
    {
        int overlayWidth = overlayImage.GetWidth();
        int overlayHeight = overlayImage.GetHeight();

        int backgroundWidth = backgroundImage.GetWidth();
        int backgroundHeight = backgroundImage.GetHeight();

        for (int x = position.X, i = 0; x < overlayWidth + position.X; x++, i++)
        {
            for (int y = position.Y, j = 0; y < overlayHeight + position.Y; y++, j++)
            {
                // Check for out of bounds.
                if (x < 0 || x >= backgroundWidth || y < 0 || y >= backgroundHeight)
                    continue;

                var overlayPixel = overlayImage.GetPixel(i, j);
                var backgroundPixel = backgroundImage.GetPixel(x, y);

                Color blendedPixel = ColorUtils.BlendColors(backgroundPixel, overlayPixel, blendMode);

                backgroundImage.SetPixel(x, y, blendedPixel);
            }
        }
    }
}