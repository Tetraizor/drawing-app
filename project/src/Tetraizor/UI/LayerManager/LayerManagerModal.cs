using Godot;
using System;
using System.Collections.Generic;
using Tetraizor.Data;
using Tetraizor.Managers;

namespace Tetraizor.UI.LayerManager;

public partial class LayerManagerModal : Control
{
    public bool IsOn => _isOn;
    private bool _isOn = false;

    [Export] private int _gap = 12;

    [Export] private TextureButton _toggleButton;
    [Export] private Control _layerContainer;

    [Export] private PackedScene _layerCardScene;

    private List<LayerCard> _layerCardList = new();

    private CanvasManager _canvasManager;

    private const float OnHeight = 300;
    private const float OffHeight = 100;

    [Export] private Button _addLayerButton;

    #region Signals

    [Signal] public delegate void LayerSelectedEventHandler(int canvasIndex, int layerIndex);

    #endregion

    #region Godot Methods

    public override void _Ready()
    {
        _toggleButton.Pressed += () => ToggleModal();
        _addLayerButton.Pressed += OnAddLayerButtonPressed;

        _canvasManager = CanvasManager.Instance;

        if (_canvasManager != null)
        {
            _canvasManager.LayerCreated += OnLayerCreated;
            _canvasManager.LayerDeleted += OnLayerDeleted;

            CallDeferred(MethodName.ForceUpdateLayerCards);
        }
    }

    #endregion

    private void OnLayerSelected(int layerIndex)
    {
        EmitSignal(nameof(LayerSelectedEventHandler), layerIndex);
    }

    private void OnAddLayerButtonPressed()
    {
        _canvasManager.CreateLayer();
    }

    private void OnCanvasDeleted(int canvasIndex)
    {
        throw new NotImplementedException();
    }

    private void OnLayerDeleted(int layerIndex)
    {
        ForceUpdateLayerCards();
    }

    private void OnLayerCreated(int layerIndex)
    {
        var layer = _canvasManager.Layers[layerIndex];
        var layerCard = _layerCardScene.Instantiate<LayerCard>();

        layerCard.Setup(layer);
        _layerContainer.AddChild(layerCard);

        layerCard.Position = new Vector2(_layerContainer.Size.X * -1, -(OffHeight + _gap));
        layerCard.SetDeferred("size", new Vector2(_layerContainer.Size.X, layerCard.Size.Y));

        _layerCardList.Add(layerCard);

        SortLayers();
    }

    private void SortLayers()
    {
        int layerCount = _layerCardList.Count;
        float currentPos = 0;

        _layerCardList.Sort((a, b) => a.AssignedLayer.Index.CompareTo(b.AssignedLayer.Index));

        for (int i = 0; i < layerCount; i++)
        {
            var layerCard = _layerCardList[i];
            MoveLayerCard(layerCard, new Vector2(0, currentPos));

            currentPos += (layerCard.IsOn ? OnHeight : OffHeight) + _gap;
        }

        // Change the MarginContainer of the layer container, which will expand its height accordingly.
        _layerContainer.GetParent<Control>().SetDeferred(
            "custom_minimum_size",
            new Vector2(0,
            currentPos)
        );

        _layerContainer.SetDeferred(
            "custom_minimum_size",
            new Vector2(0,
            currentPos)
        );
    }

    private void ForceUpdateLayerCards()
    {
        foreach (var layerCard in _layerCardList)
        {
            layerCard.QueueFree();
        }

        _layerCardList.Clear();

        var layerList = _canvasManager.Layers;
        foreach (var layer in layerList)
        {
            OnLayerCreated(layer.Index);
        }

        SortLayers();
    }

    private void MoveLayerCard(LayerCard layerCard, Vector2 targetPos)
    {
        var tween = GetTree().CreateTween();
        tween.SetParallel(true);
        tween.SetEase(Tween.EaseType.InOut);

        tween.TweenProperty(
            layerCard,
            "position",
            targetPos,
            .2f
        );

        tween.TweenProperty(
            layerCard,
            "size",
            new Vector2(_layerContainer.Size.X, (layerCard.IsOn ? OnHeight : OffHeight)),
            .2f
        );
    }

    private void OnCanvasCreated(int canvasIndex)
    {
        throw new NotImplementedException();
    }

    public void ToggleModal()
    {
        ToggleModal(!_isOn);
    }

    public void ToggleModal(bool isOn)
    {
        _isOn = isOn;

        // TODO: Change this to a tween.
        Visible = _isOn;
    }
}
