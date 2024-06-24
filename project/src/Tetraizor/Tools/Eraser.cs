using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Drawing;
using Tetraizor.Managers;
using Tetraizor.Utils;

namespace Tetraizor.Tools;

public class Eraser : Tool
{
    private DrawManager _drawManager;
    private CameraManager _cameraManager;

    private Image _eraserShape = new();

    private bool _isDrawing = false;

    private int radius;

    private List<Stroke> _tempStrokes = new List<Stroke>();

    public Eraser(int radius)
    {
        _drawManager = NodeManager.FindNodeOfType<DrawManager>();
        _cameraManager = NodeManager.FindNodeOfType<CameraManager>();

        ChangeShape(radius);
    }

    public void ChangeShape(int radius)
    {
        this.radius = radius;

        var points = GraphicsUtils.GetFilledCirclePoints(radius).ToList<Vector2I>();

        int extendedRadius = (radius * 2) + 1;

        _eraserShape.SetData(extendedRadius, extendedRadius, false, Image.Format.Rgba8, new byte[extendedRadius * extendedRadius * 4]);
        for (int x = 0; x < extendedRadius; x++)
        {
            for (int y = 0; y < extendedRadius; y++)
            {
                if (points.Contains(new Vector2I(x, y)))
                    _eraserShape.SetPixel(x, y, new Color(0, 0, 0, 1));
            }
        }
    }

    public override void StartInput(Vector2 position, float pressure)
    {
        _isDrawing = true;
        _tempStrokes.Clear();

        Stroke stroke = new Stroke((Vector2I)_cameraManager.ScreenToWorldPosition(position), pressure);
        _tempStrokes.Add(stroke);

        _drawManager.DrawTextureToBuffer(_eraserShape, stroke.Position - Vector2I.One * radius, DrawManager.DrawMode.Keep, ColorUtils.BlendMode.Erase);
    }

    public override void ContinueInput(Vector2 position, float pressure)
    {
        if (!_isDrawing) return;

        Stroke stroke = new Stroke((Vector2I)_cameraManager.ScreenToWorldPosition(position), pressure);
        _tempStrokes.Add(stroke);

        if (_tempStrokes.Count > 1)
        {
            Stroke lastStroke = _tempStrokes[_tempStrokes.Count - 2];

            var points = GraphicsUtils.GetBresenhamsPoints(lastStroke.Position, stroke.Position);

            foreach (var point in points)
            {
                _drawManager.DrawTextureToBuffer(_eraserShape, point - Vector2I.One * radius, DrawManager.DrawMode.Keep, ColorUtils.BlendMode.Erase);
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