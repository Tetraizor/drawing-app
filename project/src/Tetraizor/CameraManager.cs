using System.Diagnostics;
using Godot;

public partial class CameraManager : Camera2D
{
    private bool _isMoving = false;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Middle && mouseButton.IsPressed())
            {
                _isMoving = true;
            }

            if (mouseButton.ButtonIndex == MouseButton.Middle && mouseButton.IsReleased())
            {
                _isMoving = false;
            }

            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                Zoom *= 1.1f;
            }

            if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                Zoom /= 1.1f;
            }
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            if (_isMoving)
            {
                Position -= mouseMotion.Relative / Zoom;
            }
        }
    }

    public Vector2 ScreenToWorldPosition(Vector2 screenPosition)
    {
        Vector2 cameraSize = GetViewportRect().Size / Zoom;
        Vector2 relativePosition = GetScreenCenterPosition() - cameraSize / 2;
        return screenPosition / Zoom + relativePosition;
    }
}
