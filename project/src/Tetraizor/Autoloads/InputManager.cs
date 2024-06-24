using System;
using Godot;
using Tetraizor.CommonTypes;
using Tetraizor.Managers;
using Tetraizor.UI;
using Tetraizor.Utils;

namespace Tetraizor.Autoloads;

public partial class InputManager : AutoloadBase<InputManager>
{
    private Tool _currentTool;
    private CameraManager _cameraManager;

    private Vector2 _cursorPosition;

    private bool _isUsingTool = false;

    public override void _Ready()
    {
        _currentTool = new Tools.Brush(5);

        _cameraManager = this.FindNodeOfType<CameraManager>();

        NodeManager.FindNodeOfType<CanvasUIManager>().ToolChanged += OnToolChanged;
    }

    private void OnToolChanged(ToolType tool)
    {
        GD.Print("New tool: " + tool.ToString());

        switch (tool)
        {
            case ToolType.Brush:
                _currentTool = new Tools.Brush(20);
                break;
            case ToolType.Pen:
                break;
            case ToolType.Eraser:
                _currentTool = new Tools.Eraser(15);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.IsPressed())
                {
                    StartInput(mouseButton.Position, 1);
                }

                if (mouseButton.IsReleased())
                {
                    EndInput(mouseButton.Position, 1);
                }
            }
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            ContinueInput(mouseMotion.Position, mouseMotion.Pressure);
        }
    }

    private void StartInput(Vector2 position, float pressure)
    {
        _currentTool.StartInput(position, pressure);
    }

    private void ContinueInput(Vector2 position, float pressure)
    {
        _currentTool.ContinueInput(position, pressure);
    }

    private void EndInput(Vector2 position, float pressure)
    {
        _currentTool.EndInput(position, pressure);
    }
}
