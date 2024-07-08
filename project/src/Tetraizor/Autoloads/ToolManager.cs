using System;
using System.Collections.Generic;
using Godot;
using Tetraizor.CommonTypes;
using Tetraizor.Drawing;
using Tetraizor.Tools;

namespace Tetraizor.Autoloads;

public partial class ToolManager : AutoloadBase<ToolManager>
{
    public Tool CurrentTool => _currentTool;
    private Tool _currentTool;

    public ToolType CurrentToolType => _currentTool.ToolType;

    public Dictionary<ToolType, Tool> Tools => _tools;
    private Dictionary<ToolType, Tool> _tools;

    [Signal] public delegate void ToolChangedEventHandler(ToolType toolType);

    public override void _Ready()
    {
        base._Ready();

        InputManager.Instance.PrimaryPressBegin += OnPrimaryPressBegin;
        InputManager.Instance.PrimaryPressEnd += OnPrimaryPressEnd;
        InputManager.Instance.PrimaryPressDrag += OnPrimaryPressDrag;
        InputManager.Instance.PrimaryPressCanceled += OnPrimaryPressCanceled;

        CallDeferred(MethodName.InitializeTools);
    }

    private void InitializeTools()
    {
        _tools = new Dictionary<ToolType, Tool>
        {
            { ToolType.Brush, new Brush(5) },
            { ToolType.Eraser, new Eraser(5) },
            { ToolType.Pen, new Eraser(5) },
        };

        ChangeTool(ToolType.Brush);
    }

    private void OnPrimaryPressCanceled()
    {
    }


    private void OnPrimaryPressDrag(Vector2 position, float pressure)
    {
        _currentTool.DragInput(position, pressure);
    }


    private void OnPrimaryPressEnd(Vector2 position, float pressure)
    {
        _currentTool.EndInput(position, pressure);
    }


    private void OnPrimaryPressBegin(Vector2 position, float pressure)
    {
        _currentTool.BeginInput(position, pressure);
    }


    public void ChangeTool(ToolType toolType)
    {
        if (_currentTool != null && _currentTool.ToolType == toolType)
            return;

        _currentTool = Tools[toolType];

        EmitSignal(SignalName.ToolChanged, (int)toolType);
    }
}
