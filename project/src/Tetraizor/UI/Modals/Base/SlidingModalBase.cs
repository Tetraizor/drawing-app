namespace Tetraizor.UI.Modals.Base;

using Godot;

public partial class SlidingModalBase : ModalBase
{
    [ExportGroup("Slide Settings")]
    [Export] protected Curve _transitionCurve = ResourceLoader.Load<Curve>("res://res/default/curve/smooth_step.tres");

    [Export] private Vector2 _closedPosition;
    [Export] private Vector2 _openedPosition;

    [Export] private float _slideDuration = .25f;

    protected override void Open()
    {
        base.Open();

        var tween = GetTree().CreateTween();

        tween.TweenMethod(Callable.From((float progress) =>
        {
            MoveModal(_closedPosition, _openedPosition, progress);
        }), 0.0f, 1.0f, _slideDuration);
    }

    protected override void Close()
    {
        base.Close();

        var tween = GetTree().CreateTween();

        tween.TweenMethod(Callable.From((float progress) =>
        {
            MoveModal(_openedPosition, _closedPosition, progress);
        }), 0.0f, 1.0f, _slideDuration);
    }

    private void MoveModal(Vector2 initialPosition, Vector2 targetPosition, float progress)
    {
        Position = initialPosition.Lerp(targetPosition, _transitionCurve.Sample(progress));
    }
}