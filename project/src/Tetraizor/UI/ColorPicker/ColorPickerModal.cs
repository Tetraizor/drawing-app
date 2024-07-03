using Godot;
using System;
using Tetraizor.Autoloads;
using Tetraizor.Utils;

namespace Tetraizor.UI.ColorPicker;

public partial class ColorPickerModal : TextureButton
{
    private static ColorPickerModal _instance;
    public static ColorPickerModal Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = NodeManager.Instance.FindNodeOfType<ColorPickerModal>();
            }

            return _instance;
        }
    }

    [Export] private Curve _transitionCurve;

    [Export] private MainSelectionArea _mainSelectionArea;
    [Export] private SubSelectionArea _subSelectionArea;

    private Color _primaryColor;
    public Color PrimaryColor => _primaryColor;

    private Vector3 _hsv;

    private bool _isOn = false;

    [Export] private Control _modal;
    private Vector2 _closedPosition = Vector2.Zero;

    [Export] private Control _initialColorPreview;
    [Export] private Control _selectedColorPreview;

    [Export] private Control _buttonColor;

    [Signal] public delegate void PrimaryColorChangedEventHandler(Color color);

    public override void _Ready()
    {
        _mainSelectionArea.SelectionPositionChanged += OnMainSelectionPositionChanged;
        _subSelectionArea.SelectionPositionChanged += OnSubSelectionPositionChanged;

        PrimaryColorChanged += OnPrimaryColorChanged;

        _closedPosition = _modal.Position;

        Pressed += () => ToggleModal();

        OnPrimaryColorChanged(new Color(0, 0, 0, 1));
    }

    public void ToggleModal()
    {
        ToggleModal(!_isOn);
    }

    public void ToggleModal(bool state)
    {
        if (_isOn == state) return;

        _isOn = state;

        GD.Print("isOn: " + _isOn);
        if (_isOn)
        {

            _hsv = ColorUtils.RGBtoHSV(_primaryColor);

            _mainSelectionArea.SetSelectionPosition(new Vector2(_hsv.Y, _hsv.Z));
            _subSelectionArea.SetSelectionPosition(_hsv.X);

            _initialColorPreview.Modulate = _primaryColor;
            _selectedColorPreview.Modulate = _primaryColor;

            var tween = GetTree().CreateTween();

            tween.TweenMethod(Callable.From((float progress) =>
            {
                MoveModal(
                    _closedPosition,
                    new Vector2(_closedPosition.X, (_closedPosition.Y + _modal.Size.Y) * -1),
                    progress);
            }), 0.0f, 1.0f, .25f);
        }
        else
        {
            var tween = GetTree().CreateTween();

            tween.TweenMethod(Callable.From((float progress) =>
            {
                MoveModal(
                    new Vector2(_closedPosition.X, (_closedPosition.Y + _modal.Size.Y) * -1),
                    _closedPosition,
                    progress);
            }), 0.0f, 1.0f, .25f);
        }
    }

    private void MoveModal(Vector2 initialPosition, Vector2 targetPosition, float progress)
    {
        _modal.Position = initialPosition.Lerp(targetPosition, _transitionCurve.Sample(progress));
    }

    private void OnPrimaryColorChanged(Color color)
    {
        _primaryColor = color;

        _selectedColorPreview.Modulate = color;
        _buttonColor.Modulate = color;
    }


    private void OnMainSelectionPositionChanged(Vector2 normalizedPosition)
    {
        _hsv.Y = normalizedPosition.X;
        _hsv.Z = normalizedPosition.Y;

        _primaryColor = ColorUtils.HSVtoRGB(_hsv);
        EmitSignal(SignalName.PrimaryColorChanged, _primaryColor);
    }

    private void OnSubSelectionPositionChanged(float normalizedPosition)
    {
        _mainSelectionArea.Material.Set("shader_parameter/hue", normalizedPosition);

        _hsv.X = normalizedPosition;
        _primaryColor = ColorUtils.HSVtoRGB(_hsv);
        EmitSignal(SignalName.PrimaryColorChanged, _primaryColor);
    }
}
