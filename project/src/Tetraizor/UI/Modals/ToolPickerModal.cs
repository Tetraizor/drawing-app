namespace Tetraizor.UI.Modals;

using System;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Tools;
using Tetraizor.UI.Modals.Base;
using Tetraizor.UI.ToolManagement;

public partial class ToolPickerModal : SlidingModalBase
{
    [Export] private Button _buttonAddNew;

    private ToolManager _toolManager;

    [Export] private Control _tipCardContainer;
    [Export] private PackedScene _tipCardScene;

    public override void Register()
    {
        base.Register();

        _toolManager = NodeManager.FindNodeOfType<ToolManager>();

        _toolManager.ToolTipAdded += OnToolTipAdded;
        _toolManager.ToolTipRemoved += OnToolTipRemoved;

        _buttonAddNew.Pressed += OnAddButtonPressed;

        RemakeToolList();
    }

    private void OnToolTipRemoved()
    {
        RemakeToolList();
    }

    private void OnToolTipAdded(int tipIndex)
    {
        RemakeToolList();
    }

    private void RemakeToolList()
    {
        // Clear all children
        foreach (Node child in _tipCardContainer.GetChildren())
        {
            child.QueueFree();
        }

        // Add all children
        foreach (var tip in _toolManager.TipList)
        {
            var tipCard = _tipCardScene.Instantiate<TipCard>();
            _tipCardContainer.AddChild(tipCard);

            Callable.From(() =>
            {
                tipCard.Initialize(tip);
            }).CallDeferred();
        }
    }

    private void OnAddButtonPressed()
    {
        _toolManager.AddTip(new TipData(5));
    }
}