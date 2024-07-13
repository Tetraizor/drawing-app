using Godot;

namespace Tetraizor.UI.PatternPicker;

public partial class PatternPickerModal : SlidingModal
{
    [ExportGroup("Control References")]
    [Export] private TextureButton _toggleButton;

    public override void _Ready()
    {
        base._Ready();

        _toggleButton.Pressed += () => Toggle();
    }
}