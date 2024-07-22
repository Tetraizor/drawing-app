using Godot;

namespace Tetraizor.Utils;

public class CurveUtils
{
    public readonly static Curve Linear = ResourceLoader.Load<Curve>("res://res/default/curve/linear.tres");
    public readonly static Curve EaseIn = ResourceLoader.Load<Curve>("res://res/default/curve/ease_in.tres");
    public readonly static Curve EaseOut = ResourceLoader.Load<Curve>("res://res/default/curve/ease_out.tres");
    public readonly static Curve EaseInOut = ResourceLoader.Load<Curve>("res://res/default/curve/ease_in_out.tres");
    public readonly static Curve Bounce = ResourceLoader.Load<Curve>("res://res/default/curve/bounce.tres");
    public readonly static Curve SmoothStep = ResourceLoader.Load<Curve>("res://res/default/curve/smooth_step.tres");
}