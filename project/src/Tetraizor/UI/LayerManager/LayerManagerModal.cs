namespace Tetraizor.UI.LayerManager;

using Godot;
using System.Collections.Generic;
using Tetraizor.Managers;
using Tetraizor.UI.Modals.Base;

public partial class LayerManagerModal : SlidingModalBase
{
    #region Properties
    [Export] private int _gap = 12;

    [Export] private float _layerCardHeight = 100;

    [ExportGroup("Node References")]
    [Export] private Control _background;

    [Export] private TextureButton _toggleButton;

    [Export] private Button _closeButton;
    [Export] private Button _addLayerButton;

    [Export] private Control _layerCardContainer;

    [Export] private PackedScene _layerCardScene;

    [Export] private Control _layerCardGhost;
    [Export] private Control _dividerGhost;

    // References
    private List<LayerCard> _layerCardList = new();
    private CanvasManager _canvasManager;

    // State
    private bool _isDraggingCard = false;
    private int _draggingIndex = -1;
    private int _currentIndex = -1;
    private Vector2 _draggingOffset;
    #endregion

    #region Signals

    [Signal] public delegate void LayerSelectedEventHandler(int canvasIndex, int layerIndex);

    #endregion

    #region Godot Methods
    public override void _Ready()
    {
        _toggleButton.Pressed += () => Toggle();
        _addLayerButton.Pressed += OnAddLayerButtonPressed;
        _closeButton.Pressed += () => Toggle(false);

        _canvasManager = CanvasManager.Instance;

        if (_canvasManager != null)
        {
            _canvasManager.LayerCreated += OnLayerCreated;
            _canvasManager.LayerDeleted += OnLayerDeleted;

            CallDeferred(MethodName.ForceUpdateLayerCards);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (_isDraggingCard)
        {
            if (@event is InputEventMouseMotion mouseMotion)
            {
                _layerCardGhost.GlobalPosition = mouseMotion.Position - _draggingOffset;

                bool isInBounds =
                    (mouseMotion.Position.X >= _background.GlobalPosition.X && mouseMotion.Position.X <= _background.GlobalPosition.X + _background.Size.X) &&
                    (mouseMotion.Position.Y >= _background.GlobalPosition.Y && mouseMotion.Position.Y <= _background.GlobalPosition.Y + _background.Size.Y);

                if (!isInBounds)
                {
                    CancelLayerCardMovement();
                }

                var relativePosition = mouseMotion.Position - _layerCardContainer.GlobalPosition;

                _currentIndex = Mathf.Clamp(Mathf.RoundToInt(relativePosition.Y / (_layerCardHeight + _gap)), 0, _layerCardList.Count);
                _dividerGhost.Position = new Vector2(0, (_currentIndex * (_layerCardHeight + _gap)) - (_gap / 2)) + (_layerCardContainer.GlobalPosition - _dividerGhost.GetParent<Control>().GlobalPosition);

                _currentIndex = _canvasManager.Layers.Count - _currentIndex;
            }
            else if (@event is InputEventMouseButton mouseButton)
            {
                if (mouseButton.ButtonIndex == MouseButton.Left && !mouseButton.Pressed)
                {
                    DropLayerCard();
                }
            }
        }
    }
    #endregion

    #region Layer Management
    private void OnLayerSelected(int layerIndex)
    {
        EmitSignal(SignalName.LayerSelected, layerIndex);
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
        _layerCardContainer.AddChild(layerCard);

        layerCard.Position = new Vector2(_layerCardContainer.Size.X * -1, -(_layerCardHeight + _gap));
        layerCard.SetDeferred("size", new Vector2(_layerCardContainer.Size.X, layerCard.Size.Y));

        _layerCardList.Add(layerCard);

        layerCard.HeldDown += () => StartDraggingLayerCard(layer.Index);

        SortLayers();
    }
    #endregion

    #region Control Methods
    private void OnAddLayerButtonPressed()
    {
        _canvasManager.CreateLayer();
    }

    private void SortLayers()
    {
        int layerCount = _layerCardList.Count;
        float height = layerCount * (_layerCardHeight + _gap) - _gap;

        // Change the MarginContainer of the layer container, which will expand its height accordingly.
        _layerCardContainer.GetParent<Control>().Set(
            "custom_minimum_size",
            new Vector2(0,
            height)
        );

        _layerCardContainer.Set(
            "custom_minimum_size",
            new Vector2(0,
            height)
        );

        float currentPos = height;

        _layerCardList.Sort((a, b) => a.AssignedLayer.Index.CompareTo(b.AssignedLayer.Index));

        for (int i = 0; i < layerCount; i++)
        {
            var layerCard = _layerCardList[i];
            MoveLayerCard(layerCard, new Vector2(0, currentPos - _layerCardHeight));

            currentPos -= _layerCardHeight + _gap;
        }
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
            new Vector2(_layerCardContainer.Size.X, _layerCardHeight),
            .2f
        );
    }

    public void StartDraggingLayerCard(int index)
    {
        _canvasManager.SelectLayer(index);

        _isDraggingCard = true;
        _draggingIndex = index;

        _layerCardGhost.Visible = true;
        _dividerGhost.Visible = true;

        _layerCardGhost.Size = _layerCardList[_draggingIndex].Size;
        _dividerGhost.Size = new Vector2(_layerCardContainer.Size.X, _dividerGhost.Size.Y);

        _draggingOffset = GetViewport().GetMousePosition() - _layerCardList[_draggingIndex].GlobalPosition;
        _dividerGhost.Position =
            new Vector2(0, ((_canvasManager.Layers.Count - _draggingIndex) * (_layerCardHeight + _gap)) - (_gap / 2) + _dividerGhost.Size.Y) +
            (_layerCardContainer.GlobalPosition - _dividerGhost.GetParent<Control>().GlobalPosition);

        _layerCardGhost.GlobalPosition = _layerCardList[_draggingIndex].GlobalPosition;
    }

    public void DropLayerCard()
    {
        _currentIndex = Mathf.Clamp(_currentIndex - (_currentIndex > _draggingIndex ? 1 : 0), 0, _layerCardList.Count - 1);

        if (_draggingIndex == _currentIndex)
        {
            CancelLayerCardMovement();
            return;
        }

        _canvasManager.MoveLayer(_draggingIndex, _currentIndex);
        SortLayers();

        _isDraggingCard = false;
        _draggingIndex = -1;

        _layerCardGhost.Visible = false;
        _dividerGhost.Visible = false;

        _draggingOffset = Vector2.Zero;
    }

    private void CancelLayerCardMovement()
    {
        _isDraggingCard = false;
        _draggingIndex = -1;

        _layerCardGhost.Visible = false;
        _dividerGhost.Visible = false;

        _draggingOffset = Vector2.Zero;
    }
    #endregion
}
