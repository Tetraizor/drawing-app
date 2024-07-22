namespace Tetraizor.UI.Modals;

using System;
using Godot;
using Tetraizor.Data;
using Tetraizor.UI.Modals.Base;

public partial class LayerPropertiesModal : FullScreenModalBase
{
    [Export] private LineEdit _layerNameLineEdit;

    [Export] private Button _acceptButton;
    [Export] private Button _cancelButton;
    [Export] private Button _closeButton;

    private LayerData _layerToEdit;

    public override void Register()
    {
        base.Register();

        _acceptButton.Pressed += Save;
        _cancelButton.Pressed += Close;
        _closeButton.Pressed += Close;
    }

    public void StartEditing(LayerData layerData)
    {
        _layerToEdit = layerData;

        _layerNameLineEdit.Text = _layerToEdit.DisplayName;

        Toggle(true);
    }

    private void Save()
    {
        _layerToEdit.ChangeName(_layerNameLineEdit.Text);

        Close();
    }
}