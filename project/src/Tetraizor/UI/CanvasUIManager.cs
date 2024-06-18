using Godot;
using Tetraizor.Utils;

namespace Tetraizor.UI;

public partial class CanvasUIManager : Node
{
    [Export] private TextureButton _buttonSettings;

    public override void _Ready()
    {
        _buttonSettings.Pressed += () =>
        {
            this.FindNodeOfType<DebugUIManager>().TogglePanelMain(true);
        };
    }
}
