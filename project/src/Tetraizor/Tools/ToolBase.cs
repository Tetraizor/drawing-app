namespace Tetraizor.Tools;

using System;
using Godot;
using Tetraizor.CommonTypes;

public abstract class ToolBase
{
    public abstract ToolType ToolType { get; protected set; }

    public virtual void Initialize() { }

    public abstract void BeginInput(Vector2 position, float pressure);
    public abstract void DragInput(Vector2 position, float pressure);
    public abstract void EndInput(Vector2 position, float pressure);

    public virtual void CancelInput() { }
}