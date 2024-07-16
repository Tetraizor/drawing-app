using System;
using Godot;
using Tetraizor.CommonTypes;

public abstract class Tool
{
    public abstract ToolType ToolType { get; protected set; }

    public abstract void BeginInput(Vector2 position, float pressure);
    public abstract void DragInput(Vector2 position, float pressure);
    public abstract void EndInput(Vector2 position, float pressure);

    public virtual void CancelInput() { }
}