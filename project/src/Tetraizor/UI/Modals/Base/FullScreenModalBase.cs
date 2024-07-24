namespace Tetraizor.UI.Modals.Base;

using Godot;
using Tetraizor.Utils;


public partial class FullScreenModalBase : ModalBase
{
    [Export] private Panel _background;
    [Export] private Control _panel;

    private Color _backgroundColor;

    [Export] private float _transitionDuration = .3f;
    [Export] private float _maxBlurAmount = 2f;

    private Curve _linearCurve = CurveUtils.Linear;
    [Export] private Curve _transitionCurve = CurveUtils.Bounce;

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

        tween.Finished += () =>
        {
            MouseFilter = MouseFilterEnum.Pass;
            _isTransitioning = false;
        };

        tween.TweenMethod(Callable.From((float progress) =>
        {
            _panel.AnchorBottom = Mathf.Lerp(-0.5f, 0.5f, _transitionCurve.Sample(progress));
            _panel.AnchorTop = Mathf.Lerp(-0.5f, 0.5f, _transitionCurve.Sample(progress));

            _background.Material.Set("shader_parameter/blur_amount", Mathf.Lerp(0, _maxBlurAmount, _transitionCurve.Sample(progress)));

            _background.Material.Set("shader_parameter/tint_color", ColorUtils.Lerp(Colors.Transparent, _backgroundColor, _linearCurve.Sample(progress)));
        }), 0.0f, 1.0f, _transitionDuration);

        tween.Play();
    }

    protected override void Close()
    {
        base.Close();

        _isTransitioning = true;

        var tween = GetTree().CreateTween();
        tween.SetParallel(true);

        tween.Finished += () =>
        {
            MouseFilter = MouseFilterEnum.Ignore;
            _isTransitioning = false;

            _panel.Hide();
            _background.Hide();
        };

        tween.TweenMethod(Callable.From((float progress) =>
        {
            _panel.AnchorBottom = Mathf.Lerp(0.5f, -0.5f, _transitionCurve.Sample(progress));
            _panel.AnchorTop = Mathf.Lerp(0.5f, -0.5f, _transitionCurve.Sample(progress));

            _background.Material.Set("shader_parameter/blur_amount", Mathf.Lerp(_maxBlurAmount, 0, _transitionCurve.Sample(progress)));

            _background.Material.Set("shader_parameter/tint_color", ColorUtils.Lerp(_backgroundColor, Colors.Transparent, _linearCurve.Sample(progress)));
        }), 0.0f, 1.0f, _transitionDuration);

        tween.Play();
    }
}