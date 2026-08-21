using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameTimeSpan))]
public class GameTimeSpanPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty totalMillisecondsProperty = property.FindPropertyRelative("totalMilliseconds");
        if (totalMillisecondsProperty == null)
        {
            EditorGUI.LabelField(position, label.text, "Invalid GameTimeSpan property");
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.showMixedValue = totalMillisecondsProperty.hasMultipleDifferentValues;

        string currentText = new GameTimeSpan(totalMillisecondsProperty.intValue).ToString();
        EditorGUI.BeginChangeCheck();
        string newText = EditorGUI.DelayedTextField(position, label, currentText);

        if (EditorGUI.EndChangeCheck())
        {
            if (string.IsNullOrWhiteSpace(newText))
            {
                totalMillisecondsProperty.intValue = 0;
            }
            else if (GameTimeSpan.TryParse(newText, out GameTimeSpan parsedValue))
            {
                totalMillisecondsProperty.intValue = parsedValue.totalMilliseconds;
            }
        }

        EditorGUI.showMixedValue = false;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
