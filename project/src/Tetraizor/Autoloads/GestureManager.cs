namespace Tetraizor.Autoloads;

using System;
using Godot;

public partial class GestureManager : AutoloadBase<GestureManager>
{
    [Signal] public delegate void PanEventHandler(Vector2 delta);
    [Signal] public delegate void ZoomEventHandler(float delta);
    [Signal] public delegate void RotateEventHandler(float delta);

    private Vector2I _screenSize;

    private Vector2 _firstTouchStart;
    private Vector2 _secondTouchStart;

    private Vector2 _firstTouchLast;
    private Vector2 _secondTouchLast;

    public override void _Ready()
    {
        base._Ready();

        ApplicationManager.Instance.ScreenSizeChanged += OnScreenSizeChanged;
        _screenSize = ApplicationManager.Instance.ScreenSize;

        InputManager.Instance.GestureBegin += OnGestureBegin;
        InputManager.Instance.GestureEnd += OnGestureEnd;
        InputManager.Instance.GestureDrag += OnGestureDrag;
    }

    private void OnScreenSizeChanged(Vector2I newSize)
    {
        _screenSize = newSize;
    }

    private void OnGestureDrag(Vector2 first, Vector2 second)
    {
        float distanceDeltaInPixels = (_firstTouchLast - _secondTouchLast).Length() - (_firstTouchStart - _secondTouchStart).Length();
        float distanceDeltaInUnits = distanceDeltaInPixels / (_screenSize.Y > _screenSize.X ? _screenSize.Y : _screenSize.X);

        _firstTouchLast = first;
        _secondTouchLast = second;

        if (Mathf.Abs(distanceDeltaInUnits) > .01f)
            EmitSignal(SignalName.Zoom, distanceDeltaInPixels);
    }

    private void OnGestureEnd(Vector2 first, Vector2 second)
    {
    }

    private void OnGestureBegin(Vector2 first, Vector2 second)
    {
        _firstTouchStart = first;
        _secondTouchStart = second;

        _firstTouchLast = first;
        _secondTouchLast = second;
    }
}