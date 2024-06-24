using Godot;

public abstract class Tool
{
    public abstract void StartInput(Vector2 position, float pressure);
    public abstract void ContinueInput(Vector2 position, float pressure);
    public abstract void EndInput(Vector2 position, float pressure);
}