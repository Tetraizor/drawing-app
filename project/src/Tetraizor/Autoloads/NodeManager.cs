using Godot;

namespace Tetraizor.Autoloads;

public partial class NodeManager : AutoloadBase<NodeManager>
{
    public override void _Ready()
    {
        base._Ready();
    }

    public static T FindNodeOfType<T>(Node nodeToSearch = null) where T : Node
    {
        if (nodeToSearch == null)
        {
            if (Instance != null)
            {
                nodeToSearch = Instance.GetViewport();
            }
            else
            {
                GD.PrintErr("NodeManager: Instance is null.");
                return null;
            }
        }

        if (nodeToSearch is T)
        {
            return nodeToSearch as T;
        }
        else
        {
            foreach (Node childNode in nodeToSearch.GetChildren())
            {
                if (childNode is T)
                {
                    return childNode as T;
                }
                else
                {
                    Node foundNode = FindNodeOfType<T>(childNode);

                    if (foundNode != null)
                    {
                        return foundNode as T;
                    }
                }
            }
        }

        return null;
    }
}