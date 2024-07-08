namespace Tetraizor.Utils;

using Godot;

public static class NodeUtils
{
    public static void PrintNodePath(this Node node)
    {
        GD.Print("Node Path: " + node.GetPath());
    }
}