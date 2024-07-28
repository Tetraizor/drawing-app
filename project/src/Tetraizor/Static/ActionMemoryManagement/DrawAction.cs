namespace Tetraizor.Autoloads.ActionMemoryManagement;

using Godot;
using Tetraizor.Data;
using Tetraizor.Drawing;
using Tetraizor.Managers;

public class DrawAction : IAction
{
    private LayerData _targetLayer;
    private Image _previousImage;
    private Image _currentImage;

    public DrawAction(LayerData targetLayer, Image previousImage, Image currentImage)
    {
        _targetLayer = targetLayer;
        _previousImage = previousImage;
        _currentImage = currentImage;

        _previousImage.SetPixel(ActionMemoryManager.UndoCount, 0, new Color(1, 0, 0, 1));
    }

    public void Redo()
    {
        var drawManager = NodeManager.FindNodeOfType<DrawManager>();

        CanvasManager.Instance.SelectLayer(_targetLayer.Index);

        drawManager.DrawTextureToBuffer(_currentImage, Vector2I.Zero, DrawManager.DrawMode.Override, Utils.ColorUtils.BlendMode.Normal);
        drawManager.SaveBuffer();
        drawManager.ClearBuffer();
    }

    public void Undo()
    {
        var drawManager = NodeManager.FindNodeOfType<DrawManager>();

        CanvasManager.Instance.SelectLayer(_targetLayer.Index);

        drawManager.DrawTextureToBuffer(_previousImage, Vector2I.Zero, DrawManager.DrawMode.Override, Utils.ColorUtils.BlendMode.Normal);
        drawManager.SaveBuffer();
        drawManager.ClearBuffer();
    }
}