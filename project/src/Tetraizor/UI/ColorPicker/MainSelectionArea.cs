using System;
using Godot;

namespace Tetraizor.UI.ColorPicker;

public partial class MainSelectionArea : Control
{
    [Export] private Control _cursor;
    private Vector2 _cursorDefaultPosition;

    private bool _isSelecting = false;
    private bool _isOnControl = false;

    private Vector2 _normalizedPosition = Vector2.Zero;

    [Signal] public delegate void SelectionPositionChangedEventHandler(Vector2 normalizedPosition);

    public override void _Ready()
    {
        _cursorDefaultPosition = _cursor.Position;
        SelectionPositionChanged += OnSelectionPositionChanged;
    }

    private void OnSelectionPositionChanged(Vector2 normalizedPosition)
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.Pressed)
                {
                    _isOnControl = (mouseButton.Position.X >= GlobalPosition.X && mouseButton.Position.X <= GlobalPosition.X + Size.X) &&
                                    (mouseButton.Position.Y >= GlobalPosition.Y && mouseButton.Position.Y <= GlobalPosition.Y + Size.Y);

                    ToggleCursor(true);
                    MoveCursor(mouseButton.Position);
                }
                else
                {
                    ToggleCursor(false);
                    MoveCursor(mouseButton.Position);
                }
            }
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            MoveCursor(mouseMotion.Position);
        }
    }

    private void ToggleCursor(bool state)
    {
        _isSelecting = state;
    }

    private void MoveCursor(Vector2 position)
    {
        if (!_isSelecting || !_isOnControl) return;

        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(position.X - GlobalPosition.X, 0, Size.X),
            Mathf.Clamp(position.Y - GlobalPosition.Y, 0, Size.Y)
        );

        _normalizedPosition = clampedPosition / Size;

        // Inverse it because the Y axis is inverted on the color picker itself.
        _normalizedPosition.Y = 1 - _normalizedPosition.Y;
        EmitSignal(nameof(SelectionPositionChanged), _normalizedPosition);

        _cursor.Position = clampedPosition + _cursorDefaultPosition;
    }

    public void SetSelectionPosition(Vector2 vector2)
    {
        _cursor.Position = new Vector2(vector2.X * Size.X, vector2.Y * Size.Y) + _cursorDefaultPosition;
    }
}
