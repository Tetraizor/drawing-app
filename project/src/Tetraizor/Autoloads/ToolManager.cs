using Godot;
using Tetraizor.CommonTypes;

namespace Tetraizor.Autoloads;

public partial class ToolManager : AutoloadBase<ToolManager>
{
    public ToolType CurrentTool => _currentTool;
    private ToolType _currentTool = ToolType.None;

    [Signal] public delegate void ToolChangedEventHandler(ToolType toolType);

    public void ChangeTool(ToolType toolType)
    {
        if (CurrentTool == toolType) return;
        _currentTool = toolType;

        EmitSignal(SignalName.ToolChanged, (int)toolType);
    }
}
