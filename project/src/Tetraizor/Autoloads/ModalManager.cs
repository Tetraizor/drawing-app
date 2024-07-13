namespace Tetraizor.Autoloads;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Tetraizor.UI.ControlHelpers;
using Tetraizor.UI.Modals;

public partial class ModalManager : AutoloadBase<ModalManager>
{
    private List<ModalBase> _modals = new();

    private ModalBase _focusedModal;
    public ModalBase FocusedModal
    {
        get => _focusedModal;
        private set
        {
            _focusedModal = value;
            FocusChanged?.Invoke(_focusedModal);
        }
    }

    [Export] private EmptySpaceClickDetector _emptySpaceClickDetector;

    public delegate void FocusChangedEventHandler(ModalBase modal);
    public event FocusChangedEventHandler FocusChanged;

    public override void _Ready()
    {
        base._Ready();

        CallDeferred(MethodName.RegisterModals);
    }

    private void RegisterModals()
    {
        _modals = NodeManager.FindAllNodesOfType<ModalBase>().ToList();

        _modals.ForEach(modal =>
        {
            modal.Register();
            modal.Closed += () => OnModalClosed(modal);
            modal.Opened += () => OnModalOpened(modal);
        });

        _emptySpaceClickDetector = NodeManager.FindNodeOfType<EmptySpaceClickDetector>();
        _emptySpaceClickDetector.EmptySpacePressed += OnEmptySpacePressed;
    }

    private void OnEmptySpacePressed()
    {
        FocusedModal?.Toggle(false);
    }

    public void OnModalOpened(ModalBase modal)
    {
        if (FocusedModal == null)
        {
            FocusedModal = modal;
            FocusChanged?.Invoke(FocusedModal);
        }
        else
        {
            if (FocusedModal != modal)
            {
                // Don't close it if the already opened modal is a parent of the new modal
                if (!_focusedModal.Children.Contains(modal))
                {
                    FocusedModal.Toggle(false);
                }

                FocusedModal = modal;
            }
        }
    }

    public void OnModalClosed(ModalBase modal)
    {
        if (FocusedModal == modal)
        {
            FocusedModal = null;
            FocusChanged?.Invoke(null);
        }
    }
}