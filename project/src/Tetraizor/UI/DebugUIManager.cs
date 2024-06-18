using Godot;
using System;

namespace Tetraizor.UI;

public partial class DebugUIManager : Node
{
    # region Control References
    [ExportGroup("Control References")]
    [Export] private Control _panelMain;

    [Export] private Slider _sliderScale;
    [Export] private Label _labelScaleFeedback;

    [Export] private Button _buttonCloseMainPanel;
    # endregion

    # region States
    private bool _isPanelMainVisible = true;
    public bool IsPanelMainVisible => _isPanelMainVisible;
    #endregion

    # region Godot Methods
    public override void _Ready()
    {
        _sliderScale.DragEnded += OnSliderScaleDragEnded;
        _sliderScale.ValueChanged += OnSliderScaleValueChanged;

        _buttonCloseMainPanel.Pressed += () => TogglePanelMain(false);

        TogglePanelMain(false, 0);
    }


    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Pressed)
            {
                switch (keyEvent.Keycode)
                {
                    case Key.F:
                        GetTree().Root.ContentScaleFactor = GetTree().Root.ContentScaleFactor + .1f;
                        break;

                    case Key.G:
                        GetTree().Root.ContentScaleFactor = GetTree().Root.ContentScaleFactor - .1f;
                        break;
                }
            }
        }
    }
    #endregion

    # region Callbacks 
    private void OnSliderScaleValueChanged(double value)
    {
        _labelScaleFeedback.Text = $"{(value * 100).ToString("0.00")}%";
    }

    private void OnSliderScaleDragEnded(bool valueChanged)
    {
        double value = _sliderScale.Value;

        GetTree().Root.ContentScaleFactor = (float)value;
    }
    #endregion

    # region Panel Controls
    public void TogglePanelMain(bool state, float duration = .3f)
    {
        if (state == _isPanelMainVisible) return;

        _isPanelMainVisible = state;

        if (_isPanelMainVisible)
        {
            var tween = GetTree().CreateTween();
            tween.SetTrans(Tween.TransitionType.Cubic);
            tween.TweenProperty(_panelMain, (String)Control.PropertyName.AnchorLeft, 0, duration);
            tween.Parallel().TweenProperty(_panelMain, (String)Control.PropertyName.AnchorRight, 1, duration);
        }
        else
        {
            var tween = GetTree().CreateTween();
            tween.SetTrans(Tween.TransitionType.Cubic);
            tween.TweenProperty(_panelMain, (String)Control.PropertyName.AnchorLeft, -1, duration);
            tween.Parallel().TweenProperty(_panelMain, (String)Control.PropertyName.AnchorRight, 0, duration);
        }
    }

    public void TogglePanelMain(float duration = .3f)
    {
        TogglePanelMain(!_isPanelMainVisible, duration);
    }
    #endregion
}
