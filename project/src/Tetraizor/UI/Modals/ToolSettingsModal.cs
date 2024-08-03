namespace Tetraizor.UI.Modals;

using Godot;
using Tetraizor.Autoloads;
using Tetraizor.Tools;
using Tetraizor.UI.Modals.Base;

public partial class ToolSettingsModal : FullScreenModalBase
{
    [ExportGroup("ControlReferences")]
    [Export] private Button _buttonClose;
    [Export] private Button _buttonSave;
    [Export] private Button _buttonCancel;
    [Export] private Button _buttonRemove;

    [Export] private Slider _sliderBrushSize;
    [Export] private Label _labelBrushSizeFeedback;

    private TipData _originalTip;
    private TipData _copyTip;

    public override void Register()
    {
        _buttonSave.Pressed += () => FinishEditing();
        _buttonClose.Pressed += () => CancelEditing();
        _buttonCancel.Pressed += () => CancelEditing();
        _buttonRemove.Pressed += () => Remove();

        _sliderBrushSize.DragEnded += (size) => SyncTipData();
        _sliderBrushSize.ValueChanged += (size) => SyncTipData();

        base.Register();
    }

    private void Remove()
    {
        ToolManager.Instance.RemoveTip(_originalTip);

        CancelEditing();
    }

    public void StartEditing(TipData tipData)
    {
        _originalTip = tipData;
        _copyTip = tipData.Clone() as TipData;

        _sliderBrushSize.Value = _copyTip.Size;

        _buttonRemove.Disabled = ToolManager.Instance.TipList.Count == 1;

        SyncTipData();
        Toggle(true);
    }

    private void SyncTipData()
    {
        _labelBrushSizeFeedback.Text = _copyTip.Size.ToString();
        _copyTip.SetTipSize((int)_sliderBrushSize.Value);
    }

    protected override void Close()
    {
        base.Close();

        CancelEditing();
    }

    public void CancelEditing()
    {
        _copyTip = null;
        _originalTip = null;

        Toggle(false);
    }

    public void FinishEditing()
    {
        _originalTip.SetAll(_copyTip);

        ToolManager.Instance.SelectTip(_originalTip);

        _copyTip = null;
        _originalTip = null;

        Toggle(false);
    }
}