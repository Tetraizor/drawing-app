using Godot;

namespace Tetraizor.Utils;

public static class NodeExtensions
{
    public static void PrintNodePath(this Node node)
    {
        GD.Print("Node Path: " + node.GetPath());
    }

    public static T FindNodeOfType<T>(this Node node, Node nodeToSearch = null) where T : Node
    {
        if (nodeToSearch == null)
        {
            nodeToSearch = node.GetViewport();
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
                    Node foundNode = FindNodeOfType<T>(childNode, childNode);

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