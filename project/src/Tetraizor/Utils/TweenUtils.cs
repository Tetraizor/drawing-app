using System;
using Godot;

public static class TweenUtils
{
    public static Tween TweenMethodWithCurve(this Node node, Action<float> action, float duration, Curve curve)
    {
        var tween = node.GetTree().CreateTween();

        tween.TweenMethod(Callable.From((float progress) =>
        {
            action(curve.Sample(progress));
        }), 0.0f, 1.0f, duration);

        return tween;
    }
}