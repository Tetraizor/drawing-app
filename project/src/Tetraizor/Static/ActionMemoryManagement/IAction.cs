namespace Tetraizor.Autoloads.ActionMemoryManagement;

public interface IAction
{
    public void Undo();
    public void Redo();
}