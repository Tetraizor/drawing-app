namespace Tetraizor.UI.Modals;

using Godot;
using System;

using Tetraizor.UI.Modals.Base;

public partial class MainMenuModal : FullScreenModalBase
{
    [ExportGroup("Control References")]
    [Export] private Button _buttonClose;

    [Export] private Slider _sliderUIScale;

    [Export] private Label _labelUIScaleFeedback;

    public override void Register()
    {
        base.Register();

        // Assign Control callbacks.
        _buttonClose.Pressed += () => Toggle(false);

        _sliderUIScale.ValueChanged += OnSliderUIScaleValueChanged;
        _sliderUIScale.DragEnded += OnSliderUIScaleDragEnded;

        // Sync default values.
        _sliderUIScale.Value = GetTree().Root.ContentScaleFactor;
        OnSliderUIScaleValueChanged(_sliderUIScale.Value);
    }

    private void OnSliderUIScaleDragEnded(bool valueChanged)
    {
        double value = _sliderUIScale.Value;
        GetTree().Root.ContentScaleFactor = (float)value;
    }

    private void OnSliderUIScaleValueChanged(double value)
    {
        _labelUIScaleFeedback.Text = $"{(value * 100).ToString("0.")}%";
    }
}
