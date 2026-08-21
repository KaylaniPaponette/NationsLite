using UnityEngine.InputSystem;

public interface IInputMapHandler { }


public abstract class InputMap
{
    protected readonly InputActionMap actionMap;

    public InputMap(string name)
    {
        actionMap = InputSystem.actions.FindActionMap(name);
    }

    public abstract void SetHandler(IInputMapHandler handler);
}

public class InputMap<T> : InputMap
    where T : class, IInputMapHandler
{
    public T handler { get; private set; }

    public InputMap(string name)
        : base(name)
    { }

    public override void SetHandler(IInputMapHandler handler)
    {
        this.handler = handler as T;
        if (this.handler != null)
            actionMap.Enable();
        else
            actionMap.Disable();
    }
}
