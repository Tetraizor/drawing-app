using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.CommonTypes;
using Tetraizor.Drawing;
using Tetraizor.Managers;
using Tetraizor.UI.ColorPicker;
using Tetraizor.Utils;

namespace Tetraizor.Tools;

public class Brush : TippedToolBase
{
    #region References
    private DrawManager _drawManager;
    private CameraManager _cameraManager;
    #endregion

    #region Brush Properties
    private Image _shape;
    private Color _color;
    #endregion

    #region Tool State
    bool _isDrawing = false;
    #endregion

    private List<Stroke> _tempStrokes = new List<Stroke>();
    public override ToolType ToolType { get; protected set; } = ToolType.Brush;

    #region Base Methods
    public override void Initialize()
    {
        base.Initialize();

        _cameraManager = NodeManager.FindNodeOfType<CameraManager>();
        _drawManager = NodeManager.FindNodeOfType<DrawManager>();

    }
    #endregion

    public override void ChangeTip(TipData tipData)
    {
        base.ChangeTip(tipData);
        _color = ColorPickerModal.Instance.PrimaryColor;
    }

    protected override void OnTipDataChanged()
    {
        _shape = _currentTip.Shape;
    }

    #region Input Callbacks
    public override void BeginInput(Vector2 position, float pressure)
    {
        _isDrawing = true;
        _tempStrokes.Clear();

        Stroke stroke = new Stroke((Vector2I)_cameraManager.ScreenToWorldPosition(position), pressure);
        _tempStrokes.Add(stroke);

        _drawManager.DrawTextureToBuffer(_shape, stroke.Position - new Vector2I(_currentTip.Size, _currentTip.Size), DrawManager.DrawMode.Keep, ColorUtils.BlendMode.Alpha);
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

            var points = GraphicsUtils.GetBresenhamsPoints(lastStroke.Position, stroke.Position, _currentTip.Size).ToList();

            foreach (var point in points)
            {
                _drawManager.DrawTextureToBuffer(_shape, point - new Vector2I(_currentTip.Size, _currentTip.Size), DrawManager.DrawMode.Keep, ColorUtils.BlendMode.Alpha);
            }
        }
    }

    public override void EndInput(Vector2 position, float pressure)
    {
        _isDrawing = false;

        _drawManager.FinishStroke();
    }

    public override void CancelInput()
    {
        _isDrawing = false;

        _drawManager.ClearBuffer();
    }
    #endregion
}