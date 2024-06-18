using Godot;
using System.Collections.Generic;
using Tetraizor.Managers;
using Tetraizor.Utils;

namespace Tetraizor.Drawing;

public partial class DrawManager : Control
{
    private List<Stroke> _tempStrokes = new List<Stroke>();
    private List<Brush> _brushes = new List<Brush>() {
        new Brush(10)
    };

    private bool _isDrawing = false;

    [Export] private CameraManager _cameraManager;
    [Export] private TextureRect _canvas;

    // Actual texture that holds all the drawing.
    private ImageTexture _renderedTexture;

    // Temporary texture that only holds the latest drawing.
    private Image _strokeImage;

    // Image that holds the actual drawing.
    private Image _renderedImage;

    private Image _bufferImage;

    public override void _Ready()
    {
        _renderedImage = new Image();
        _renderedImage.SetData(512, 512, false, Image.Format.Rgba8, new byte[512 * 512 * 4]);
        _renderedImage.Fill(Colors.White);

        _strokeImage = new Image();
        _strokeImage.SetData(512, 512, false, Image.Format.Rgba8, new byte[512 * 512 * 4]);
        _strokeImage.Fill(Colors.Transparent);

        _bufferImage = new Image();
        _bufferImage.SetData(512, 512, false, Image.Format.Rgba8, new byte[512 * 512 * 4]);
        _bufferImage.Fill(Colors.Transparent);

        _renderedTexture = ImageTexture.CreateFromImage(_renderedImage);

        _canvas.Texture = _renderedTexture;

        Input.UseAccumulatedInput = false;

        StartStroke(new Vector2(0, 0));
        DrawLine(new Vector2I(0, 0), new Vector2I(512, 256));
        EndStroke();

        StartStroke(new Vector2(0, 0));
        DrawLine(new Vector2I(0, 0), new Vector2I(0, 256));
        EndStroke();

        StartStroke(new Vector2(0, 0));
        DrawLine(new Vector2I(0, 0), new Vector2I(256, 0));
        EndStroke();

        StartStroke(new Vector2(0, 0));
        DrawLine(new Vector2I(0, 0), new Vector2I(128, 256));
        EndStroke();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.IsPressed())
                {
                    StartStroke(mouseButton.Position);
                }

                if (mouseButton.IsReleased())
                {
                    EndStroke();
                }
            }
        }

        if (@event is InputEventMouseMotion mouseMotion)
        {
            Vector2 worldPosition = _cameraManager.ScreenToWorldPosition(mouseMotion.Position) - GlobalPosition;
            _cursorPosition = worldPosition;

            if (_isDrawing)
            {
                var stroke = new Stroke((Vector2I)worldPosition, mouseMotion.Pressure + .5f);
                AddStroke(stroke);
            }
        }
    }

    private void DrawLine(Vector2I start, Vector2I end)
    {
        Vector2I[] points = GraphicsUtils.GetBresenhamsPoints(start, end);

        for (int i = 1; i < points.Length - 1; i++)
        {
            Stroke stroke = new Stroke(points[i], 1);
            AddStroke(stroke, false);
        }
    }

    private void AddStroke(Stroke stroke, bool checkForGaps = true)
    {
        if (checkForGaps)
        {
            if (_tempStrokes.Count > 0)
            {
                Stroke lastStroke = _tempStrokes[_tempStrokes.Count - 1];
                float distance = (stroke.Position - lastStroke.Position).Length();

                if (distance > 1.5f)
                {
                    DrawLine(lastStroke.Position, stroke.Position);
                }
            }
        }

        _tempStrokes.Add(stroke);
        Brush brush = _brushes[0];

        int xStart = stroke.Position.X - (int)(brush.Size / 2);
        int yStart = stroke.Position.Y - (int)(brush.Size / 2);

        for (int y = yStart; y < yStart + brush.Size; y++)
        {
            for (int x = xStart; x < xStart + brush.Size; x++)
            {
                if (x < 0 || x >= 512 || y < 0 || y >= 512)
                    continue;

                Color brushColor = new Color(1, .5f, 1, 0.8f);

                _strokeImage.SetPixel(x, y, brushColor);

                Color bufferedColor = _bufferImage.GetPixel(x, y);
                Color mixedColor = bufferedColor.Blend(brushColor);

                _bufferImage.SetPixel(x, y, mixedColor);
            }
        }
    }

    private void StartStroke(Vector2 position)
    {
        _isDrawing = true;

        _bufferImage.SetData(512, 512, false, _renderedImage.GetFormat(), _renderedImage.GetData());
        _strokeImage.SetData(512, 512, false, _renderedImage.GetFormat(), new byte[512 * 512 * 4]);

        _strokeImage.Fill(Colors.Transparent);

        var stroke = new Stroke((Vector2I)(_cameraManager.ScreenToWorldPosition(position) - GlobalPosition), .5f);
        _tempStrokes.Add(stroke);
    }

    private void EndStroke()
    {
        _isDrawing = false;
        _tempStrokes.Clear();

        _renderedImage.CopyFrom(_bufferImage);

        _strokeImage.Fill(Colors.Transparent);
        _bufferImage.Fill(Colors.Transparent);

    }

    private Vector2 _cursorPosition = new Vector2();

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    private Image MixImages(Image bottom, Image top)
    {
        Image mixedImage = new Image();
        mixedImage.SetData(512, 512, false, Image.Format.Rgba8, new byte[512 * 512 * 4]);

        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                Color bottomColor = bottom.GetPixel(x, y);
                Color topColor = top.GetPixel(x, y);

                if (topColor.A == 0)
                    continue;

                Color mixedColor = bottomColor.Blend(topColor);

                bottom.SetPixel(x, y, mixedColor);
            }
        }

        return mixedImage;
    }

    public void ClearCanvas()
    {
        _renderedImage.Fill(Colors.White);
        _bufferImage.Fill(Colors.Transparent);
    }

    public override void _Draw()
    {
        Brush brush = _brushes[0];
        DrawRect(new Rect2(_cursorPosition - (Vector2.One * (brush.Size / 2)), Vector2.One * brush.Size), Colors.Red);

        if (_isDrawing)
        {
            _renderedTexture.SetImage(_bufferImage);
        }
        else
        {
            _renderedTexture.SetImage(_renderedImage);
        }
    }
}
