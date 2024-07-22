namespace Tetraizor.UI.Modals.Base;

using System.Collections.Generic;
using Godot;
using Tetraizor.Autoloads;

public abstract partial class ModalBase : Control
{
    #region Modal Access
    private static ModalBase _instance;
    public static ModalBase Instance => _instance;
    #endregion

    public bool IsOn => _isOn;
    protected bool _isOn = false;

    public ModalBase Parent => _parent;
    protected ModalBase _parent;

    public List<ModalBase> Children => _children;
    private List<ModalBase> _children = new();

    [Signal] public delegate void OpenedEventHandler();
    [Signal] public delegate void ClosedEventHandler();

    private ModalManager _modalManager;

    #region Modal Controls
    public virtual void Register()
    {
        _instance = this;

        _modalManager = ModalManager.Instance;

        _parent?._children.Add(this);
    }

    public void Toggle()
    {
        Toggle(!_isOn);
    }

    public void Toggle(bool state)
    {
        _isOn = state;

        if (_isOn) Open();
        else Close();
    }

    protected virtual void Open()
    {
        _parent?.Open();

        EmitSignal(SignalName.Opened);
    }

    protected virtual void Close()
    {
        EmitSignal(SignalName.Closed);
    }
    #endregion
}