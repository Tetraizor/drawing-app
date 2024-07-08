using Godot;

using Tetraizor.Autoloads;
using Tetraizor.UI.ControlHelpers;
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
                _instance = NodeManager.FindNodeOfType<ColorPickerModal>();
            }

            return _instance;
        }
    }

    # region Color Properties

    [ExportGroup("Node References")]
    [Export] private Curve _transitionCurve;

    [Export] private MainSelectionArea _mainSelectionArea;
    [Export] private SubSelectionArea _subSelectionArea;

    [Export] private Control _root;

    [Export] private Control _modal;

    [Export] private Control _buttonColor;

    [Export] private Control _initialColorPreview;
    [Export] private Control _selectedColorPreview;

    #endregion

    # region Color Properties
    private Color _primaryColor;
    public Color PrimaryColor => _primaryColor;

    private Vector3 _hsv;

    private bool _isOn = false;

    private Vector2 _closedPosition = Vector2.Zero;
    #endregion

    # region Signals
    [Signal] public delegate void PrimaryColorChangedEventHandler(Color color);
    #endregion

    # region Godot Methods

    public override void _Ready()
    {
        // Events
        _mainSelectionArea.SelectionPositionChanged += OnMainSelectionPositionChanged;
        _subSelectionArea.SelectionPositionChanged += OnSubSelectionPositionChanged;

        PrimaryColorChanged += OnPrimaryColorChanged;

        Pressed += () => ToggleModal();

        var emptySpaceClickDetector = NodeManager.FindNodeOfType<EmptySpaceClickDetector>(_root);
        emptySpaceClickDetector.PressedOnEmptySpace += () => ToggleModal(false);

        // Setup
        _closedPosition = _modal.Position;

        OnPrimaryColorChanged(new Color(0, 0, 0, 1));
    }

    #endregion

    # region Modal Controls 

    public void ToggleModal()
    {
        ToggleModal(!_isOn);
    }

    public void ToggleModal(bool state)
    {
        if (_isOn == state) return;

        _isOn = state;

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

    #endregion

    # region Callbacks

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

    # endregion
}