using System;

namespace Tetraizor.Autoloads.ActionMemoryManagement;

public class CustomAction : IAction
{
    private Action _redo;
    private Action _undo;

    public CustomAction(Action redo, Action undo)
    {
        _redo = redo;
        _undo = undo;
    }

    public void Redo()
    {
        _redo();
    }

    public void Undo()
    {
        _undo();
    }
}