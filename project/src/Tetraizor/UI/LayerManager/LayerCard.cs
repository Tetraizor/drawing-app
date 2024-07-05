using Godot;

using System;

using Tetraizor.Data;
using Tetraizor.Managers;

namespace Tetraizor.UI.LayerManager;

public partial class LayerCard : Control
{
    #region Control References
    [ExportGroup("Control References")]
    [Export] private TextureRect _layerPreview;
    [Export] private Label _layerNameLabel;
    #endregion

    #region Properties
    private string _layerName;
    public Layer AssignedLayer => _assignedLayer;
    private Layer _assignedLayer;

    private bool _isOn;
    public bool IsOn => _isOn;
    #endregion

    [Signal] public delegate void LayerCardPressedEventHandler();

    // TODO: Implement LayerCard.Setup better.
    public void Setup(Layer layer)
    {
        _layerName = layer.DisplayName;
        _layerNameLabel.Text = _layerName;

        _assignedLayer = layer;

        Name = _layerName;

        _layerPreview.Texture = AssignedLayer.Renderer.RenderImageTexture;

        LayerCardPressed += OnLayerCardPressed;
    }

    private void OnLayerCardPressed()
    {
        CanvasManager.Instance.SelectLayer(_assignedLayer.Index);
    }

    private bool _isStartedPressing;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                _isStartedPressing = (
                    (mouseButton.Position.X >= GlobalPosition.X && mouseButton.Position.X <= GlobalPosition.X + Size.X) &&
                    (mouseButton.Position.Y >= GlobalPosition.Y && mouseButton.Position.Y <= GlobalPosition.Y + Size.Y)
                );
            }

            if (mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed && _isStartedPressing)
            {
                if ((mouseButton.Position.X >= GlobalPosition.X && mouseButton.Position.X <= GlobalPosition.X + Size.X) &&
                    (mouseButton.Position.Y >= GlobalPosition.Y && mouseButton.Position.Y <= GlobalPosition.Y + Size.Y))
                {
                    EmitSignal(SignalName.LayerCardPressed);
                }
            }
        }
    }
}
