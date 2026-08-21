using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>, IDisposable
{
    public readonly GameplayInputMap gameplayInputMap = new GameplayInputMap();
    public readonly MouseInputMap mouseInputMap = new MouseInputMap();

    Stack<IInputMapHandler> inputHandlerStack = new Stack<IInputMapHandler>();
    bool refreshInputMaps;

    public Vector2 mousePosition => mouseInputMap.mousePosition;

    public void PushInputHandler(IInputMapHandler handler)
    {
        inputHandlerStack.Push(handler);
        refreshInputMaps = true;
    }

    public void PopInputHandler(IInputMapHandler handler)
    {
        Debug.Assert(inputHandlerStack.Peek() == handler);
        refreshInputMaps = true;
    }

    // Defer enabling/disabling input maps til next frame, so button presses
    // are not consumed by second input map.
    public void OnUpdate()
    {
        if (refreshInputMaps)
        {
            refreshInputMaps = false;

            inputHandlerStack.TryPeek(out var handler);
            gameplayInputMap.SetHandler(handler);

            // Quick hack to ensure InputManager isn't overridden as
            // default mouse handler.
            if (mouseInputMap.handler == null)
                mouseInputMap.SetHandler(handler);
        }
    }

    public void Dispose()
    {
        gameplayInputMap.Dispose();
        mouseInputMap.Dispose();
    }
}