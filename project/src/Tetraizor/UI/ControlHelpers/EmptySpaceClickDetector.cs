using Godot;

namespace Tetraizor.UI.ControlHelpers;

public partial class EmptySpaceClickDetector : Node
{
    [Signal] public delegate void PressedOnEmptySpaceEventHandler();

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                EmitSignal(SignalName.PressedOnEmptySpace);
            }
        }
    }
}
