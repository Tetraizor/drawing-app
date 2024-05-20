using Godot;
using System;

using Tetraizor.Utils;

public partial class UIManager : Node
{
    [Export] private TextureButton _penButton;
    [Export] private TextureButton _eraserButton;
    [Export] private TextureButton _clearButton;

    public override void _Ready()
    {
        _penButton.Pressed += OnPenButtonPressed;
        _eraserButton.Pressed += OnEraserButtonPressed;
        _clearButton.Pressed += OnClearButtonPressed;
    }

    private void OnClearButtonPressed()
    {
        this.FindNodeOfType<DrawManager>().ClearCanvas();
    }


    private void OnEraserButtonPressed()
    {
        throw new NotImplementedException();
    }


    private void OnPenButtonPressed()
    {
        throw new NotImplementedException();
    }


    public override void _Process(double delta)
    {
    }
}
