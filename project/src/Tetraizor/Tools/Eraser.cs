using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.CommonTypes;
using Tetraizor.Drawing;
using Tetraizor.Managers;
using Tetraizor.Utils;

namespace Tetraizor.Tools;

public class Eraser : TippedToolBase
{
    private DrawManager _drawManager;
    private CameraManager _cameraManager;

    private Image _shape = new();

    private bool _isDrawing = false;

    private List<Stroke> _tempStrokes = new List<Stroke>();

    public override ToolType ToolType { get; protected set; } = ToolType.Eraser;

    public override void Initialize()
    {
        base.Initialize();

        _drawManager = NodeManager.FindNodeOfType<DrawManager>();
        _cameraManager = NodeManager.FindNodeOfType<CameraManager>();
    }

    public void CreateShape()
    {
        var points = GraphicsUtils.GetFilledCirclePoints(_currentTip.Size).ToList();
        int radius = _currentTip.Size;

        // Calculate the diameter of the circle
        int diameter = radius * 2 + 1;

        // Create a new brush image with the correct dimensions and format
        _shape.SetData(diameter, diameter, false, Image.Format.Rgba8, new byte[diameter * diameter * 4]);

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                // Correctly calculate the index in the points array
                int index = y * diameter + x;

                if (points[index])
                    _shape.SetPixel(x, y, new Color(1, 1, 1, 1));
            }
        }
    }

    public override void BeginInput(Vector2 position, float pressure)
    {
        _isDrawing = true;
        _tempStrokes.Clear();

        Stroke stroke = new Stroke((Vector2I)_cameraManager.ScreenToWorldPosition(position), pressure);
        _tempStrokes.Add(stroke);

        _drawManager.DrawTextureToBuffer(_shape, stroke.Position - new Vector2I(_currentTip.Size, _currentTip.Size), DrawManager.DrawMode.Keep, ColorUtils.BlendMode.Erase);
    }

    public override void DragInput(Vector2 position, float pressure)
    {
        if (!_isDrawing) return;

        var worldPosition = (Vector2I)_cameraManager.ScreenToWorldPosition(position);

        Stroke stroke = new Stroke(worldPosition, pressure);
        _tempStrokes.Add(stroke);

        if (_tempStrokes.Count > 1)
        {
            Stroke lastStroke = _tempStrokes[_tempStrokes.Count - 2];

            var points = GraphicsUtils.GetBresenhamsPoints(lastStroke.Position, stroke.Position);

            foreach (var point in points)
            {
                _drawManager.DrawTextureToBuffer(_shape, point - new Vector2I(_currentTip.Size, _currentTip.Size), DrawManager.DrawMode.Keep, ColorUtils.BlendMode.Alpha);
            }
        }
    }

    public override void EndInput(Vector2 position, float pressure)
    {
        _isDrawing = false;

        _drawManager.SaveBuffer();
        _drawManager.ClearBuffer();
    }

    protected override void OnTipDataChanged()
    {
        CreateShape();
    }

    public override void ChangeTip(TipData tipData)
    {
        base.ChangeTip(tipData);
    }
}