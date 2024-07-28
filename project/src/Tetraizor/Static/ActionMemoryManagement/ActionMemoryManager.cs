namespace Tetraizor.Autoloads.ActionMemoryManagement;

using System.Collections.Generic;

public static class ActionMemoryManager
{
    private static FiniteStack<IAction> _undoStack = new FiniteStack<IAction>(MaxUndoCount);
    private static FiniteStack<IAction> _redoStack = new FiniteStack<IAction>(MaxUndoCount);

    public const int MaxUndoCount = 100;

    public static int UndoCount => _undoStack.Count;
    public static int RedoCount => _redoStack.Count;

    public delegate void ActionMemoryChangedEventHandler(FiniteStack<IAction> undoStack, FiniteStack<IAction> redoStack);
    public static event ActionMemoryChangedEventHandler ActionMemoryChanged;

    public static void AddAction(IAction action)
    {
        _undoStack.Push(action);
        _redoStack.Clear();

        ActionMemoryChanged?.Invoke(_undoStack, _redoStack);
    }

    public static void Undo()
    {
        if (_undoStack.Count == 0) return;

        var action = _undoStack.Pop();
        action.Undo();

        _redoStack.Push(action);

        ActionMemoryChanged?.Invoke(_undoStack, _redoStack);
    }

    public static void Redo()
    {
        if (_redoStack.Count == 0) return;

        var action = _redoStack.Pop();
        action.Redo();

        _undoStack.Push(action);

        ActionMemoryChanged?.Invoke(_undoStack, _redoStack);
    }
}
