namespace Tetraizor.UI.Modals;

using Godot;
using Tetraizor.Autoloads.ActionMemoryManagement;
using Tetraizor.Data;
using Tetraizor.UI.Components;
using Tetraizor.UI.Modals.Base;

public partial class LayerPropertiesModal : FullScreenModalBase
{
    [Export] private LineEdit _layerNameLineEdit;

    [Export] private Button _acceptButton;
    [Export] private Button _cancelButton;
    [Export] private Button _closeButton;

    [Export] private Checkbox _visibleCheckbox;
    [Export] private Checkbox _renderSmoothCheckbox;

    private LayerData _layerToEdit;

    public override void Register()
    {
        base.Register();

        _acceptButton.Pressed += Save;
        _cancelButton.Pressed += () => Toggle(false);
        _closeButton.Pressed += () => Toggle(false);

        _visibleCheckbox.Toggled += OnVisibleCheckboxToggled;
        _renderSmoothCheckbox.Toggled += OnRenderSmoothCheckboxToggled;
    }

    private void OnVisibleCheckboxToggled(bool isChecked)
    {
        ActionMemoryManager.AddAction(new CustomAction(
            () => _layerToEdit.Renderer.Visible = isChecked,
            () => _layerToEdit.Renderer.Visible = !isChecked));
        _layerToEdit.Renderer.Visible = isChecked;
    }

    private void OnRenderSmoothCheckboxToggled(bool isChecked)
    {
        ActionMemoryManager.AddAction(new CustomAction(
            () => _layerToEdit.Renderer.TextureFilter = isChecked ? TextureFilterEnum.Linear : TextureFilterEnum.Nearest,
            () => _layerToEdit.Renderer.TextureFilter = !isChecked ? TextureFilterEnum.Linear : TextureFilterEnum.Nearest));
        _layerToEdit.Renderer.TextureFilter = isChecked ? TextureFilterEnum.Linear : TextureFilterEnum.Nearest;
    }

    public void StartEditing(LayerData layerData)
    {
        _layerToEdit = layerData;

        GD.Print(_layerToEdit.Renderer.TextureFilter);

        _layerNameLineEdit.Text = _layerToEdit.DisplayName;
        _visibleCheckbox.SetState(_layerToEdit.Renderer.Visible, true, true);
        _renderSmoothCheckbox.SetState(_layerToEdit.Renderer.TextureFilter == TextureFilterEnum.Linear, true, true);

        Toggle(true);
    }

    private void Save()
    {
        _layerToEdit.ChangeName(_layerNameLineEdit.Text);

        Toggle(false);
    }
}