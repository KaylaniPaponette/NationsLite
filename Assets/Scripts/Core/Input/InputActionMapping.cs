using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InputMappingHint
{
    public string key;
    public Sprite mouseIcon;
    public Sprite keyboardIcon;
    public string displayName;
}

[CreateAssetMenu(fileName = "InputActionMapping", menuName = "SP3D/InputActionMapping")]
public class InputActionMapping : ScriptableObject
{
    public List<InputMappingHint> inputMappingHints;
}