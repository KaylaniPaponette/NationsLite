//
// Adapted from: https://discussions.unity.com/t/set-pseudo-state-style-from-c-script/883289/11
//

using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

[Flags]
public enum PseudoState
{
    Active = 1,
    Hover = 2,
    Checked = 8,
    Disabled = 0x20,
    Focus = 0x40,
    Root = 0x80
}

public static class VisualElementExtensions
{
    public static PseudoState GetPsuedoState(this VisualElement element)
    {                       
        return (PseudoState)element.GetType().GetProperty("pseudoStates", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(element);
    }

    public static void AddPsuedoState(this VisualElement element, PseudoState state)
    {
        PseudoState result = element.GetPsuedoState() | state;
        var enumType = element.GetType().GetProperty("pseudoStates", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(element).GetType();
        if (enumType != null && enumType.IsEnum)
        {
            object enumValue = Enum.ToObject(enumType, result);
            element.GetType().GetProperty("pseudoStates", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(element, enumValue);
        }
        else
        {
            Debug.Log("pseudoStates is not enum");
        }
    }

    public static void RemovePsuedoState(this VisualElement element, PseudoState state)
    {
        PseudoState result = element.GetPsuedoState();
        result &= ~state;
        var enumType = element.GetType().GetProperty("pseudoStates", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(element).GetType();
        if (enumType != null && enumType.IsEnum)
        {
            object enumValue = Enum.ToObject(enumType, result);
            element.GetType().GetProperty("pseudoStates", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(element, enumValue);
        }
        else
        {
            Debug.Log("pseudoStates is not enum");
        }
    }

    public static bool HasPseudoFlag(this VisualElement element, PseudoState flag)
    {
        PseudoState result = element.GetPsuedoState();
        return (result & flag) == flag;
    }
}
