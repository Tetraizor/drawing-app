using System;
using System.Collections.Generic;
using Godot;
using Tetraizor.CommonTypes;
using Tetraizor.Drawing;
using Tetraizor.Tools;
using Tetraizor.UI.Modals;

namespace Tetraizor.Autoloads;

public partial class ToolManager : AutoloadBase<ToolManager>
{
    public ToolBase CurrentTool => _currentTool;
    private ToolBase _currentTool;

    public ToolType CurrentToolType => _currentTool.ToolType;
    public TipData CurrentTip
    {
        get
        {
            if (_currentTool is TippedToolBase tipTool)
                return tipTool.CurrentTip;
            else
                return null;
        }
    }

    public Dictionary<ToolType, ToolBase> Tools => _tools;
    private Dictionary<ToolType, ToolBase> _tools;

    public List<TipData> TipList => _tipList;
    private List<TipData> _tipList = new List<TipData>() {
        new TipData(5),
        new TipData(10),
        new TipData(15),
        new TipData(20),
    };

    [Signal] public delegate void ToolChangedEventHandler(ToolType toolType);
    [Signal] public delegate void ToolTipChangedEventHandler(int tipIndex);

    [Signal] public delegate void ToolTipAddedEventHandler(int tipIndex);
    [Signal] public delegate void ToolTipRemovedEventHandler();

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
        _tools = new Dictionary<ToolType, ToolBase>
        {
            { ToolType.Brush, new Brush() },
            { ToolType.Eraser, new Eraser() },
            { ToolType.Pen, new Eraser() },
        };

        foreach (var tool in _tools.Values)
        {
            tool.Initialize();

            if (tool is TippedToolBase tipTool)
            {
                tipTool.ChangeTip(_tipList[0]);
            }
        }

        ChangeTool(ToolType.Brush);
    }

    public void SelectTip(int tipIndex) => SelectTip(_tipList[tipIndex]);
    public void SelectTip(TipData tipData)
    {
        if (_currentTool is not TippedToolBase)
        {
            GD.PrintErr("Current tool is not a tip tool.");
            return;
        }

        if (!_tipList.Contains(tipData))
        {
            GD.PrintErr("Tip not found in the list.");
            return;
        }

        ((TippedToolBase)_currentTool).ChangeTip(tipData);

        EmitSignal(SignalName.ToolTipChanged, _tipList.IndexOf(tipData));
    }

    public void AddTip(TipData tipData)
    {
        _tipList.Add(tipData);

        foreach (var tool in _tools.Values)
        {
            if (tool is TippedToolBase tipTool)
            {
                tipTool.ChangeTip(tipData);
            }
        }

        EmitSignal(SignalName.ToolTipAdded, _tipList.Count - 1);
    }

    public void RemoveTip(TipData tipData)
    {
        if (_tipList.Count == 1)
        {
            GD.PrintErr("Cannot remove the last tip.");
            return;
        }

        if (_tipList.Contains(tipData))
            _tipList.Remove(tipData);
        else
            return;

        foreach (var tool in _tools.Values)
        {
            if (tool is TippedToolBase tipTool)
            {
                if (tipTool.CurrentTip == tipData)
                {
                    tipTool.ChangeTip(_tipList[0]);
                }
            }
        }

        EmitSignal(SignalName.ToolTipRemoved);
    }

    private void OnPrimaryPressCanceled()
    {
        _currentTool?.CancelInput();
    }

    private void OnPrimaryPressDrag(Vector2 position, float pressure)
    {
        _currentTool?.DragInput(position, pressure);
    }

    private void OnPrimaryPressEnd(Vector2 position, float pressure)
    {
        _currentTool?.EndInput(position, pressure);
    }

    private void OnPrimaryPressBegin(Vector2 position, float pressure)
    {
        _currentTool?.BeginInput(position, pressure);
    }

    public void ChangeTool(ToolType toolType)
    {
        if (_currentTool != null && _currentTool.ToolType == toolType)
            return;

        _currentTool = Tools[toolType];
        if (_currentTool is TippedToolBase tipTool)
        {
            SelectTip(tipTool.CurrentTip);
        }

        EmitSignal(SignalName.ToolChanged, (int)toolType);
    }

    public void EditTip(TipData tip)
    {
        ModalManager.GetModal<ToolSettingsModal>().StartEditing(tip);
    }
}
