using System;
using System.Diagnostics;
using Godot;
using Tetraizor.Managers;
using Tetraizor.Utils;

namespace Tetraizor.Drawing;

public partial class DrawManager : Node
{
    public enum DrawMode { Override, Keep }
    private CanvasManager _canvasManager;

    private Canvas _currentCanvas;
    private LayerRenderer _currentLayer;

    private ImageTexture _renderTexture;

    private Image _bufferImage;

    private bool _isRectDirty = false;
    private Rect2I _dirtyRect;

    public void SelectCanvas(int canvasIndex)
    {
        try
        {
            _currentCanvas = _canvasManager.GetCanvas(canvasIndex);
            SelectLayer(1);
        }
        catch (IndexOutOfRangeException)
        {
            GD.PrintErr("DrawManager: Canvas index out of range.");
        }
    }

    public void SelectLayer(int layerIndex)
    {
        try
        {
            _currentLayer = _canvasManager.GetLayer(_currentCanvas, layerIndex);
            _renderTexture = _currentLayer.RenderImageTexture;
            _bufferImage = _currentLayer.BufferImage;
        }
        catch (IndexOutOfRangeException)
        {
            GD.PrintErr("DrawManager: Layer index out of range.");
        }
        catch (ArgumentNullException)
        {
            GD.PrintErr("DrawManager: Layer is null.");
        }
    }

    #region Godot Methods
    public override void _Ready()
    {
        GD.Print("DrawManager: Ready.");

        _canvasManager = CanvasManager.Instance;

        if (_canvasManager == null)
        {
            GD.PrintErr("DrawManager: CanvasManager is null.");
        }

        SelectCanvas(0);
    }

    public override void _Process(double delta)
    {
        // TODO: Change this with it's partial counterpart when Godot 4.3 comes.
        // TODO: Feature is not implemented yet, and is very performance heavy currently.
        if (_isRectDirty)
        {
            _currentLayer.SetDirty();
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

    private void BlendImage(Image backgroundImage, Image overlayImage, Vector2I position, ColorUtils.BlendMode blendMode)
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