namespace Tetraizor.UI.ControlHelpers;

using Godot;

public partial class EmptySpaceClickDetector : Node
{
    [Signal] public delegate void EmptySpacePressedEventHandler();

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                EmitSignal(SignalName.EmptySpacePressed);
            }
        }
    }
}
