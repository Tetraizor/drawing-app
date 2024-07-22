namespace Tetraizor.UI.Modals.Base;

using Godot;

public partial class FullScreenModalBase : ModalBase
{
    [Export] private Panel _background;
    [Export] private Control _panel;

    private Color _backgroundColor;

    [Export] private float _transitionDuration = .3f;
    [Export] private float _maxBlurAmount = 2f;

    private bool _isTransitioning = false;

    public override void Register()
    {
        base.Register();

        _background = GetNode<Panel>("./Background");

        _backgroundColor = (Color)(_background.Material as ShaderMaterial).GetShaderParameter("tint_color");

        // Close at the beginning to revert to default state.
        float duration = _transitionDuration;
        _transitionDuration = 0;
        Close();
        _transitionDuration = duration;

        Show();
    }

    protected override void Open()
    {
        base.Open();

        _background.Show();
        _panel.Show();

        _isTransitioning = true;

        var tween = GetTree().CreateTween();
        tween.SetParallel(true);

        tween.TweenProperty(_panel, "anchor_bottom", 0.5f, _transitionDuration);
        tween.TweenProperty(_panel, "anchor_top", 0.5f, _transitionDuration);

        tween.TweenProperty(_background.Material, "shader_parameter/tint_color", _backgroundColor, _transitionDuration);
        tween.TweenProperty(_background.Material, "shader_parameter/blur_amount", _maxBlurAmount, _transitionDuration);

        tween.Finished += () =>
        {
            MouseFilter = MouseFilterEnum.Pass;
            _isTransitioning = false;
        };

        tween.Play();
    }

    protected override void Close()
    {
        base.Close();

        _isTransitioning = true;

        var tween = GetTree().CreateTween();
        tween.SetParallel(true);

        tween.TweenProperty(_panel, "anchor_bottom", -0.5f, _transitionDuration);
        tween.TweenProperty(_panel, "anchor_top", -0.5f, _transitionDuration);

        tween.TweenProperty(_background.Material, "shader_parameter/tint_color", Colors.Transparent, _transitionDuration);
        tween.TweenProperty(_background.Material, "shader_parameter/blur_amount", 0, _transitionDuration);

        tween.Finished += () =>
        {
            MouseFilter = MouseFilterEnum.Ignore;
            _isTransitioning = false;

            _panel.Hide();
            _background.Hide();
        };

        tween.Play();
    }
}