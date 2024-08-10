using System;
using Godot;

namespace Tetraizor.UI.ColorPicker;

public partial class SubSelectionArea : Control
{
    [Export] private ColorPickerModal _colorPickerModal;
    [Export] private Control _cursor;

    private Vector2 _cursorDefaultPosition;

    private bool _isSelecting = false;
    private bool _isOnControl = false;

    private float _normalizedPosition = 0;

    [Signal] public delegate void SelectionPositionChangedEventHandler(float normalizedPosition);
    [Signal] public delegate void SelectionPositionFinishedChangingEventHandler(float normalizedPosition);

    public override void _Ready()
    {
        _cursorDefaultPosition = _cursor.Position;
        SelectionPositionChanged += OnSelectionPositionChanged;
    }

    private void OnSelectionPositionChanged(float normalizedPosition)
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (!_colorPickerModal.IsOn) return;

        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.Pressed)
                {
                    _isOnControl = (mouseButton.Position.X >= GlobalPosition.X && mouseButton.Position.X <= GlobalPosition.X + Size.X) &&
                                                       (mouseButton.Position.Y >= GlobalPosition.Y && mouseButton.Position.Y <= GlobalPosition.Y + Size.Y);

                    ToggleCursor(true);
                    MoveCursorToMousePosition(mouseButton.Position.Y);
                }
                else
                {
                    ToggleCursor(false);
                    MoveCursorToMousePosition(mouseButton.Position.Y);

                    if (_isOnControl)
                        EmitSignal(SignalName.SelectionPositionFinishedChanging, _normalizedPosition);
                }
            }
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            if (_isOnControl)
                MoveCursorToMousePosition(mouseMotion.Position.Y);
        }
    }

    private void ToggleCursor(bool state)
    {
        _isSelecting = state;
    }

    private void MoveCursorToMousePosition(float y)
    {
        if (!_isSelecting || !_isOnControl) return;

        Vector2 clampedPosition = new Vector2(
            0,
            Mathf.Clamp(y - GlobalPosition.Y, 0, Size.Y)
        );

        _normalizedPosition = (clampedPosition / Size).Y;
        EmitSignal(nameof(SelectionPositionChanged), _normalizedPosition);

        MoveCursor(_normalizedPosition);
    }

    private void MoveCursor(float progress)
    {
        _cursor.Position = new Vector2(_cursor.Position.X, progress * Size.Y);
    }

    public void SetSelectionPosition(float hue)
    {
        MoveCursor(hue);
    }
}
