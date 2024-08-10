namespace Tetraizor.Tools;

using System;
using System.Linq;
using Godot;
using Tetraizor.UI.ColorPicker;
using Tetraizor.UI.ToolManagement;
using Tetraizor.Utils;

public class TipData : ICloneable
{
    private int _size;
    public int Size => _size;

    private Curve _pressureCurve = CurveUtils.Linear;
    public Curve PressureCurve => _pressureCurve;

    public string Name => _name;
    private string _name;

    private Image _shape;
    public Image Shape => _shape;

    private Color _color = Colors.Black;
    public Color Color => _color;

    public delegate void TipDataChangedEventHandler();
    public event TipDataChangedEventHandler TipDataChanged;

    public TipData(int size, string name = "Brush", Curve pressureCurve = null)
    {
        // Default Assignments
        if (pressureCurve == null)
        {
            pressureCurve = CurveUtils.Linear;
        }

        _size = size;
        _pressureCurve = pressureCurve;
        _name = name;

        ColorPickerModal.Instance.ColorSelected += OnColorSelected;

        CreateShape();
    }

    private void CreateShape()
    {
        // TODO: Change brush generation. It currently only works with filled circles.
        _shape = new Image();

        var points = GraphicsUtils.GetFilledCirclePoints(Size).ToList();
        int radius = Size;

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
                {
                    _shape.SetPixel(x, y, _color);
                }
            }
        }

        TipDataChanged?.Invoke();
    }

    private void OnColorSelected(Color color)
    {
        _color = color;
        CreateShape();
    }

    public override string ToString()
    {
        return $"{_name} ({_size})";
    }

    public object Clone()
    {
        return new TipData(_size, _name, _pressureCurve);
    }

    public void SetAll(TipData tipData)
    {
        _pressureCurve = tipData.PressureCurve;
        _size = tipData.Size;
        _name = tipData.Name;

        CreateShape();
    }

    public void SetTipSize(int size)
    {
        _size = size;

        CreateShape();
    }
    public void SetPressureCurve(Curve curve)
    {
        _pressureCurve = curve;

        CreateShape();
    }
    public void SetName(string name)
    {
        _name = name;
        TipDataChanged?.Invoke();
    }
}