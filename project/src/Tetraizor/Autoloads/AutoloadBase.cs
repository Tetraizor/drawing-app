using Godot;

namespace Tetraizor.Autoloads;

public abstract partial class AutoloadBase<T> : Node where T : AutoloadBase<T>
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GD.PrintErr($"{typeof(T).Name}: Instance is null.");
                throw new System.NullReferenceException();
            }

            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    private static T _instance;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("SingletonManagerBase: Instance already exists.");
            QueueFree();
            return;
        }
        GD.Print($"{typeof(T).Name}: Instance is ready.");

        _instance = (T)this;
    }

    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}