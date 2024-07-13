namespace Tetraizor.UI.ColorPicker;

using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Utils;

public partial class ColorPickerModal : SlidingModal
{
    #region Accessor
    // TODO: Change this access method to a more centralized one.
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
    #endregion

    #region Color Properties
    [ExportGroup("Control References")]
    [Export] private Control _toggleButtonColorPreview;
    [Export] private TextureButton _toggleButton;

    [Export] private MainSelectionArea _mainSelectionArea;
    [Export] private SubSelectionArea _subSelectionArea;

    [Export] private Control _initialColorPreview;
    [Export] private Control _selectedColorPreview;
    #endregion

    #region Color Properties
    private Color _primaryColor;
    public Color PrimaryColor => _primaryColor;

    private Vector3 _hsv;
    #endregion

    #region Signals
    [Signal] public delegate void PrimaryColorChangedEventHandler(Color color);
    #endregion

    #region Godot Methods
    public override void _Ready()
    {
        _mainSelectionArea.SelectionPositionChanged += OnMainSelectionPositionChanged;
        _subSelectionArea.SelectionPositionChanged += OnSubSelectionPositionChanged;

        PrimaryColorChanged += OnPrimaryColorChanged;

        _toggleButton.Pressed += () => Toggle();

        OnPrimaryColorChanged(new Color(0, 0, 0, 1));
    }
    #endregion

    #region Modal Controls
    protected override void Open()
    {
        base.Open();

        _hsv = ColorUtils.RGBtoHSV(_primaryColor);

        _mainSelectionArea.SetSelectionPosition(new Vector2(_hsv.Y, _hsv.Z));
        _subSelectionArea.SetSelectionPosition(_hsv.X);

        _initialColorPreview.Modulate = _primaryColor;
        _selectedColorPreview.Modulate = _primaryColor;
    }

    protected override void Close()
    {
        base.Close();
    }
    #endregion

    #region Callbacks
    private void OnPrimaryColorChanged(Color color)
    {
        _primaryColor = color;

        _selectedColorPreview.Modulate = color;
        _toggleButtonColorPreview.Modulate = color;
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
    #endregion
}