using System;
using System.Collections.Generic;

using Godot;
using Tetraizor.Autoloads;

using Tetraizor.CommonTypes;
using Tetraizor.Managers;
using Tetraizor.Utils;

namespace Tetraizor.UI;

public partial class CanvasUIManager : Node
{
    [Export] private TextureButton _buttonSettings;
    [Export] private TextureButton _buttonBrush;
    [Export] private TextureButton _buttonPen;
    [Export] private TextureButton _buttonEraser;

    private const float TransitionDuration = .05f;

    private Dictionary<ToolType, ToolButton> _toolButtons = new();
    public Dictionary<ToolType, ToolButton> ToolButtons => _toolButtons;

    private ToolManager _toolManager;

    [Signal] public delegate void ToolChangedEventHandler(ToolType tool);

    public override void _Ready()
    {
        GD.Print("CanvasUIManager: Ready.");

        // Get references.
        _toolManager = ToolManager.Instance;

        // Assign Control event callbacks.
        _buttonSettings.Pressed += () =>
        {
            this.FindNodeOfType<DebugUIManager>().TogglePanelMain(true);
        };

        RegisterButton(ToolType.Brush, () => ChangeTool(ToolType.Brush), _buttonBrush);
        RegisterButton(ToolType.Pen, () => ChangeTool(ToolType.Pen), _buttonPen);
        RegisterButton(ToolType.Eraser, () => ChangeTool(ToolType.Eraser), _buttonEraser);

        ChangeTool(ToolType.Brush);
    }

    private void ChangeTool(ToolType tool)
    {
        if (_toolManager.CurrentTool == tool) return;
        _toolManager.ChangeTool(tool);

        foreach (var toolButton in _toolButtons.Values)
        {
            if (_toolManager.CurrentTool == toolButton.Tool)
            {
                toolButton.Button.TextureNormal = toolButton.ActiveTexture;
                toolButton.ActiveIconTextureRect.Visible = true;
                toolButton.PassiveIconTextureRect.Visible = false;

                var tween = GetTree().CreateTween();
                tween.SetTrans(Tween.TransitionType.Cubic);
                tween.TweenProperty(toolButton.Button, (String)Control.PropertyName.Scale, Vector2.One * 1.1f, TransitionDuration);
                tween.TweenProperty(toolButton.Button, (String)Control.PropertyName.Scale, Vector2.One, TransitionDuration);

                EmitSignal(SignalName.ToolChanged, (int)toolButton.Tool);
            }
            else
            {
                toolButton.Button.TextureNormal = toolButton.PassiveTexture;
                toolButton.ActiveIconTextureRect.Visible = false;
                toolButton.PassiveIconTextureRect.Visible = true;
            }
        }
    }

    private void RegisterButton(ToolType tool, Action action, TextureButton button)
    {
        var toolButton = new ToolButton
        {
            Tool = tool,
            Button = button,

            PassiveTexture = button.TextureDisabled,
            ActiveTexture = button.TextureNormal,

            PassiveIconTextureRect = button.GetChild<TextureRect>(0),
            ActiveIconTextureRect = button.GetChild<TextureRect>(1),
        };

        button.Pressed += action;

        _toolButtons.Add(tool, toolButton);
    }

    public struct ToolButton
    {
        public ToolType Tool { get; set; }
        public TextureButton Button { get; set; }

        public Texture2D ActiveTexture { get; set; }
        public Texture2D PassiveTexture { get; set; }

        public TextureRect ActiveIconTextureRect { get; set; }
        public TextureRect PassiveIconTextureRect { get; set; }
    }
}
