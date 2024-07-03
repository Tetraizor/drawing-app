using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Drawing;
using Tetraizor.Managers;
using Tetraizor.UI.ColorPicker;
using Tetraizor.Utils;

namespace Tetraizor.Tools;

public class Brush : Tool
{
    private DrawManager _drawManager;
    private CameraManager _cameraManager;

    private Image _brushImage;

    bool _isDrawing = false;

    private int _radius = 5;

    private List<Stroke> _tempStrokes = new List<Stroke>();

    public Brush(int radius)
    {
        this._radius = radius;

        ColorPickerModal.Instance.PrimaryColorChanged += (color) => CreateShape();

        CreateShape();

        _drawManager = NodeManager.FindNodeOfType<DrawManager>();
        _cameraManager = NodeManager.FindNodeOfType<CameraManager>();
    }

    public void CreateShape()
    {
        _brushImage = new Image();
        Color brushColor = ColorPickerModal.Instance.PrimaryColor;

        var points = GraphicsUtils.GetFilledCirclePoints(_radius).ToList();

        int extendedRadius = (_radius * 2) + 1;

        _brushImage.SetData(extendedRadius, extendedRadius, false, Image.Format.Rgba8, new byte[extendedRadius * extendedRadius * 4]);
        for (int x = 0; x < extendedRadius; x++)
        {
            for (int y = 0; y < extendedRadius; y++)
            {
                if (points.Contains(new Vector2I(x, y)))
                    _brushImage.SetPixel(x, y, brushColor);
            }
        }
    }

    public override void StartInput(Vector2 position, float pressure)
    {
        _isDrawing = true;
        _tempStrokes.Clear();

        Stroke stroke = new Stroke((Vector2I)_cameraManager.ScreenToWorldPosition(position), pressure);
        _tempStrokes.Add(stroke);

        _drawManager.DrawTextureToBuffer(_brushImage, stroke.Position - new Vector2I(_radius, _radius), DrawManager.DrawMode.Keep, ColorUtils.BlendMode.Alpha);

    }

    public override void ContinueInput(Vector2 position, float pressure)
    {
        if (!_isDrawing) return;

        var worldPosition = (Vector2I)_cameraManager.ScreenToWorldPosition(position);

        Stroke stroke = new Stroke(worldPosition, pressure);
        _tempStrokes.Add(stroke);

        if (_tempStrokes.Count > 1)
        {
            Stroke lastStroke = _tempStrokes[_tempStrokes.Count - 2];

            var points = GraphicsUtils.GetBresenhamsPoints(lastStroke.Position, stroke.Position).ToList();

            foreach (var point in points)
            {
                _drawManager.DrawTextureToBuffer(_brushImage, point - new Vector2I(_radius, _radius), DrawManager.DrawMode.Keep, ColorUtils.BlendMode.Alpha);
            }
        }
    }

    public override void EndInput(Vector2 position, float pressure)
    {
        _isDrawing = false;

        _drawManager.SaveBuffer();
        _drawManager.ClearBuffer();
    }
}