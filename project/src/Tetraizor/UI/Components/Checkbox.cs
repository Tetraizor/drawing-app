namespace Tetraizor.UI.Components;

using Godot;
using Tetraizor.Utils;


public partial class Checkbox : Control
{
    [Export] private Button _buttonClickArea;
    [Export] private TextureRect _iconCheck;

    [Signal] public delegate void ToggledEventHandler(bool isChecked);

    [Export] private bool _checked = false;
    public bool Checked => _checked;

    public override void _Ready()
    {
        _buttonClickArea.Pressed += OnButtonPressed;
        SetState(_checked, true, true);
    }

    public void SetState(bool state, bool muted = false, bool force = false)
    {
        if (!force && state == _checked) return;

        _checked = state;

        Vector2 targetScale = _checked ? Vector2.One : Vector2.Zero;
        Vector2 currentScale = _iconCheck.Scale;

        var tween = this.TweenMethodWithCurve((float progress) =>
        {
            var calculatedScale = Vector2Utils.Lerp(currentScale, targetScale, progress);
            _iconCheck.Scale = calculatedScale;
        }, _checked ? .15f : .05f, _checked ? CurveUtils.Bounce : CurveUtils.EaseOut);

        if (_checked) _iconCheck.Visible = true;
        else tween.Finished += () => _iconCheck.Visible = false;

        if (!muted) EmitSignal(SignalName.Toggled, _checked);
    }

    private void OnButtonPressed()
    {
        SetState(!_checked);
    }
}
