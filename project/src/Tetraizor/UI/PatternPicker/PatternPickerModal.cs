using Godot;
using Tetraizor.UI.Modals.Base;

namespace Tetraizor.UI.PatternPicker;

public partial class PatternPickerModal : SlidingModalBase
{
    [ExportGroup("Control References")]
    [Export] private TextureButton _toggleButton;

    public override void _Ready()
    {
        base._Ready();

        _toggleButton.Pressed += () => Toggle();
    }
}