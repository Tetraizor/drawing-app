using Godot;

using System;

using Tetraizor.Data;
using Tetraizor.Managers;

namespace Tetraizor.UI.LayerManager;

public partial class LayerCard : PanelContainer
{
    #region Control References
    [ExportGroup("Control References")]
    [Export] private TextureRect _layerPreview;
    [Export] private Label _layerNameLabel;
    [Export] private Button _optionsButton;

    private float _pressTime = 0.5f;
    #endregion

    #region Properties
    private string _layerName;

    public LayerData AssignedLayer => _assignedLayer;
    private LayerData _assignedLayer;

    private bool _isOn;
    public bool IsOn => _isOn;

    private bool _isPressing;

    [Export] private StyleBoxFlat _style;

    [Export] private Color _onColor;
    [Export] private Color _offColor;
    #endregion

    [Signal] public delegate void PressedEventHandler();
    [Signal] public delegate void HeldDownEventHandler();

    // TODO: Implement LayerCard.Setup better.
    public void Setup(LayerData layer)
    {
        CanvasManager.Instance.LayerSelected += OnSelected;
        _optionsButton.Pressed += OnOptionsPressed;
        Pressed += OnPressed;

        _layerName = layer.DisplayName;
        _layerNameLabel.Text = _layerName;

        _assignedLayer = layer;

        Name = _layerName;

        _layerPreview.Texture = AssignedLayer.Renderer.RenderImageTexture;
    }

    private void OnOptionsPressed()
    {
        GD.Print("Options pressed");
    }

    private void OnSelected(int layerIndex)
    {
        if (layerIndex == _assignedLayer.Index)
        {
            _isOn = true;

            var tween = GetTree().CreateTween();
            tween.TweenProperty(_style, "bg_color", _onColor, .2f);
        }
        else
        {
            _isOn = false;

            var tween = GetTree().CreateTween();
            tween.TweenProperty(_style, "bg_color", _offColor, .2f);
        }
    }

    private void OnPressed()
    {
        CanvasManager.Instance.SelectLayer(_assignedLayer.Index);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                _isPressing = (
                    (mouseButton.Position.X >= GlobalPosition.X && mouseButton.Position.X <= GlobalPosition.X + Size.X) &&
                    (mouseButton.Position.Y >= GlobalPosition.Y && mouseButton.Position.Y <= GlobalPosition.Y + Size.Y)
                );

                if (_isPressing)
                {
                    var timer = new Timer
                    {
                        WaitTime = _pressTime,
                        Autostart = true,
                        OneShot = true,
                    };

                    timer.Timeout += () =>
                    {
                        if (_isPressing)
                        {
                            EmitSignal(SignalName.HeldDown);
                        }

                        timer.QueueFree();
                    };

                    AddChild(timer);
                }
            }

            if (mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed && _isPressing)
            {
                if ((mouseButton.Position.X >= GlobalPosition.X && mouseButton.Position.X <= GlobalPosition.X + Size.X) &&
                    (mouseButton.Position.Y >= GlobalPosition.Y && mouseButton.Position.Y <= GlobalPosition.Y + Size.Y))
                {
                    EmitSignal(SignalName.Pressed);
                }

                _isPressing = false;
            }
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            if (_isPressing)
            {
                if ((mouseMotion.Position.X < GlobalPosition.X || mouseMotion.Position.X > GlobalPosition.X + Size.X) ||
                    (mouseMotion.Position.Y < GlobalPosition.Y || mouseMotion.Position.Y > GlobalPosition.Y + Size.Y))
                {
                    _isPressing = false;
                }
            }
        }
    }
}
