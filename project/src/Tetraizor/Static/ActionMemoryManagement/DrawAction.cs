namespace Tetraizor.Autoloads.ActionMemoryManagement;

using Godot;
using Tetraizor.Drawing;

public class DrawAction : IAction
{
    private ImageTexture _targetTexture;
    private Image _previousImage;
    private Image _currentImage;

    public DrawAction(ImageTexture renderTexture, Image previousImage, Image currentImage)
    {
        _targetTexture = renderTexture;
        _previousImage = previousImage;
        _currentImage = currentImage;

        _previousImage.SetPixel(ActionMemoryManager.UndoCount, 0, new Color(1, 0, 0, 1));
    }

    public void Redo()
    {
        var drawManager = NodeManager.FindNodeOfType<DrawManager>();

        drawManager.DrawTextureToBuffer(_currentImage, Vector2I.Zero, DrawManager.DrawMode.Override, Utils.ColorUtils.BlendMode.Normal);
        drawManager.SaveBuffer();
        drawManager.ClearBuffer();
    }

    public void Undo()
    {
        var drawManager = NodeManager.FindNodeOfType<DrawManager>();

        drawManager.DrawTextureToBuffer(_previousImage, Vector2I.Zero, DrawManager.DrawMode.Override, Utils.ColorUtils.BlendMode.Normal);
        drawManager.SaveBuffer();
        drawManager.ClearBuffer();
    }
}