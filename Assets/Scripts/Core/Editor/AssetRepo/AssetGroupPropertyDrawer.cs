using UnityEngine;
using UnityEditor;
using System.IO;

[CustomPropertyDrawer(typeof(AssetGroup))]
public class AssetGroupDrawer : PropertyDrawer
{
    private const float ButtonWidth = 70f;
    private const float Spacing = 5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Get the folderName and assets properties
        SerializedProperty folderNameProp = property.FindPropertyRelative("folderName");
        SerializedProperty assetsProp = property.FindPropertyRelative("assets");
        SerializedProperty autoGenerateKeysProp = property.FindPropertyRelative("useNumberedKey");

        // Use BeginProperty/EndProperty to handle prefab overrides, record undo/redo, etc.
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            float currentY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            float contentWidth = position.width;

            // Draw autoGenerateKeys toggle and label in one row
            Rect toggleRect = new Rect(position.x, currentY, 18f, EditorGUIUtility.singleLineHeight);
            Rect toggleLabelRect = new Rect(toggleRect.xMax + Spacing, currentY, 150f, EditorGUIUtility.singleLineHeight);
            autoGenerateKeysProp.boolValue = EditorGUI.Toggle(toggleRect, autoGenerateKeysProp.boolValue);
            EditorGUI.LabelField(toggleLabelRect, "Use Numbered Key");
            currentY += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Draw folder name row with button
            float folderLabelWidth = 70f;
            float folderFieldWidth = contentWidth - folderLabelWidth - ButtonWidth - 2 * Spacing;
            Rect folderLabelRect = new Rect(position.x, currentY, folderLabelWidth, EditorGUIUtility.singleLineHeight);
            Rect folderRect = new Rect(folderLabelRect.xMax + Spacing, currentY, folderFieldWidth, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new Rect(folderRect.xMax + Spacing, currentY, ButtonWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(folderLabelRect, "Folder");
            string folderLabel = string.IsNullOrEmpty(folderNameProp.stringValue) ? "(No folder selected)" : folderNameProp.stringValue;
            EditorGUI.LabelField(folderRect, folderLabel);

            if (GUI.Button(buttonRect, "Browse"))
            {
                string projectPath = Application.dataPath;
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", projectPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        selectedPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                    folderNameProp.stringValue = selectedPath;
                    property.serializedObject.ApplyModifiedProperties();
                    GUIUtility.ExitGUI();
                }
            }
            currentY += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Draw assets array
            if (assetsProp != null)
            {
                Rect assetsRect = new Rect(position.x, currentY, contentWidth, EditorGUI.GetPropertyHeight(assetsProp, true));
                EditorGUI.PropertyField(assetsRect, assetsProp, new GUIContent("Assets"), true);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight; // Base height for foldout
        if (property.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // toggle
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // folder row
            SerializedProperty assetsProp = property.FindPropertyRelative("assets");
            height += EditorGUI.GetPropertyHeight(assetsProp, true); // assets array
        }
        return height;
    }
}