namespace Tetraizor.Tools;

using System;
using Godot;
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

        TipDataChanged?.Invoke();
    }

    public void SetTipSize(int size)
    {
        _size = size;
        TipDataChanged?.Invoke();
    }
    public void SetPressureCurve(Curve curve)
    {
        _pressureCurve = curve;
        TipDataChanged?.Invoke();
    }
    public void SetName(string name)
    {
        _name = name;
        TipDataChanged?.Invoke();
    }
}