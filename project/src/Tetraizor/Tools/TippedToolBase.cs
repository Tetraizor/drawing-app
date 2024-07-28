using System;
using Godot;

namespace Tetraizor.Tools;

public abstract class TippedToolBase : ToolBase
{
    public TipData CurrentTip => _currentTip;
    protected TipData _currentTip;

    public override void Initialize()
    {
        base.Initialize();

        if (_currentTip == null) _currentTip = new TipData(0);
        _currentTip.TipDataChanged += OnTipDataChanged;
    }

    public virtual void ChangeTip(TipData tipData)
    {
        _currentTip.TipDataChanged -= OnTipDataChanged;
        _currentTip = tipData;
        _currentTip.TipDataChanged += OnTipDataChanged;

        OnTipDataChanged();
    }

    protected abstract void OnTipDataChanged();
}