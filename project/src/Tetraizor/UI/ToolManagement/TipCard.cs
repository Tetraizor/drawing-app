namespace Tetraizor.UI.ToolManagement;

using System;
using System.Linq;
using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Drawing;
using Tetraizor.Tools;
using Tetraizor.Utils;

public partial class TipCard : Control
{
    [ExportGroup("Control References")]
    [Export] private Label _nameLabel;
    [Export] private Label _sizeLabel;

    [Export] private Button _settingsButton;

    [Export] private TextureRect _previewTextureRect = new TextureRect();

    [ExportGroup("Style References")]
    [Export] private StyleBoxFlat _styleBoxFlat;

    [Export] private Color _normalColor;
    [Export] private Color _selected;

    private TipData _tip;

    private Image _previewImage = new Image();
    private ImageTexture _previewTexture = new ImageTexture();

    private bool _isPointerOverControl = false;

    private DrawManager _drawManager;

    public void Initialize(TipData tip)
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        _settingsButton.Pressed += () => ToolManager.Instance.EditTip(tip);

        _drawManager = NodeManager.FindNodeOfType<DrawManager>();

        ToolManager.Instance.ToolTipChanged += OnToolTipChanged;

        _styleBoxFlat.Set("bg_color", _normalColor);

        _tip = tip;

        _nameLabel.Text = tip.Name;
        _sizeLabel.Text = tip.Size.ToString();

        _tip.TipDataChanged += OnTipDataChanged;

        // Manual sync for the first time, as this is added to the tree AFTER tool manager has been initialized.
        if (ToolManager.Instance.CurrentTip == tip) OnToolTipChanged(ToolManager.Instance.TipList.IndexOf(tip));

        _previewTextureRect.Texture = _previewTexture;
        UpdatePreview();

        Name = tip.Name;
    }

    private void OnTipDataChanged()
    {
        _sizeLabel.Text = _tip.Size.ToString();
        UpdatePreview();
    }

    private void OnToolTipChanged(int tipIndex)
    {
        var selectedTip = ToolManager.Instance.TipList[tipIndex];

        if (selectedTip == _tip)
        {
            _styleBoxFlat.Set("bg_color", _selected);
        }
        else
        {
            _styleBoxFlat.Set("bg_color", _normalColor);
        }
    }

    private void OnMouseExited() => _isPointerOverControl = false;
    private void OnMouseEntered() => _isPointerOverControl = true;

    public void UpdatePreview()
    {
        _previewImage.SetData(
            (int)_previewTextureRect.Size.X,
            (int)_previewTextureRect.Size.Y,
            false,
            Image.Format.Rgba8,
            new byte[(int)_previewTextureRect.Size.X * (int)_previewTextureRect.Size.Y * 4]
        );

        int height = (int)_previewTextureRect.Size.Y;
        int width = (int)_previewTextureRect.Size.X;

        var points = GraphicsUtils.GetBresenhamsPoints(new Vector2I(height / 2, 0), new Vector2I(width - (height / 2), 0), _tip.Size / 4).ToList();

        foreach (var point in points)
        {
            int offset = (int)(Mathf.Sin((float)(point.X - height / 2) / (width - height) * Mathf.Pi * 2) * (height / 4)) + height / 2;

            _drawManager.BlendImage(
                _previewImage,
                _tip.Shape,
                point - new Vector2I(_tip.Size, _tip.Size - offset),
                ColorUtils.BlendMode.Alpha);
        }

        _previewTexture.SetImage(_previewImage);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
            {
                if (_isPointerOverControl)
                {
                    // Set input handled.
                    GetViewport().SetInputAsHandled();

                    if (_tip == ToolManager.Instance.CurrentTip)
                    {
                        ToolManager.Instance.EditTip(_tip);
                    }
                    else
                    {
                        ToolManager.Instance.SelectTip(_tip);
                    }
                }
            }
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        _tip.TipDataChanged -= OnTipDataChanged;
        ToolManager.Instance.ToolTipChanged -= OnToolTipChanged;
    }
}