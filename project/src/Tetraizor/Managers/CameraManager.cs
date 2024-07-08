using System;
using Godot;
using Tetraizor.Autoloads;

namespace Tetraizor.Managers;

public partial class CameraManager : Camera2D
{
    private bool _isMoving = false;

    private Vector2I _screenSize;

    public override void _Ready()
    {
        var inputManager = InputManager.Instance;

        inputManager.Pan += OnPan;
        inputManager.Zoom += OnZoom;

        _screenSize = ApplicationManager.Instance.ScreenSize;
        ApplicationManager.Instance.ScreenSizeChanged += OnScreenSizeChanged;
    }

    private void OnScreenSizeChanged(Vector2I newSize)
    {
        _screenSize = newSize;
    }

    private void OnZoom(float delta)
    {
        Zoom = Vector2.One * Mathf.Clamp(Zoom.X + delta, .1f, 16f);
    }

    private void OnPan(Vector2 delta)
    {
        Position += delta;
    }

    public Vector2 ScreenToWorldPosition(Vector2 screenPosition)
    {
        Vector2 cameraSize = GetViewportRect().Size / Zoom;
        Vector2 relativePosition = GetScreenCenterPosition() - cameraSize / 2;

        return screenPosition / Zoom + relativePosition;
    }
}
