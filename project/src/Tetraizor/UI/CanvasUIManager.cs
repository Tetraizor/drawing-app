using System;
using System.Collections.Generic;

using Godot;
using Tetraizor.Autoloads;

using Tetraizor.CommonTypes;
using Tetraizor.UI.Modals;

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

    public override void _Ready()
    {
        // Get references.
        _toolManager = ToolManager.Instance;

        // Assign Control event callbacks.
        _buttonSettings.Pressed += () => ModalManager.GetModal<MainMenuModal>().Toggle(true);
        ToolManager.Instance.ToolChanged += (ToolType toolType) => UpdateToolButtons();

        RegisterButton(ToolType.Eraser, () => OnToolButtonPressed(ToolType.Eraser), _buttonEraser);
        RegisterButton(ToolType.Brush, () => OnToolButtonPressed(ToolType.Brush), _buttonBrush);
        RegisterButton(ToolType.Pen, () => OnToolButtonPressed(ToolType.Pen), _buttonPen);
    }

    private void OnToolButtonPressed(ToolType tool)
    {
        if (_toolManager.CurrentToolType == tool)
        {
            ModalManager.GetModal<ToolPickerModal>().Toggle();
        }
        else
        {
            ModalManager.GetModal<ToolPickerModal>().Toggle(false);
            _toolManager.ChangeTool(tool);
        }
    }

    private void UpdateToolButtons()
    {
        foreach (var toolButton in _toolButtons.Values)
        {
            if (_toolManager.CurrentToolType == toolButton.Tool)
            {
                toolButton.Button.TextureNormal = toolButton.ActiveTexture;
                toolButton.ActiveIconTextureRect.Visible = true;
                toolButton.PassiveIconTextureRect.Visible = false;

                var tween = GetTree().CreateTween();
                tween.SetTrans(Tween.TransitionType.Cubic);
                tween.TweenProperty(toolButton.Button, (String)Control.PropertyName.Scale, Vector2.One * 1.1f, TransitionDuration);
                tween.TweenProperty(toolButton.Button, (String)Control.PropertyName.Scale, Vector2.One, TransitionDuration);
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
