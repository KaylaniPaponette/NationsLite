using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AssetRepository))]
public class AssetRepositoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AssetRepository repository = (AssetRepository)target;
        if (GUILayout.Button("Populate from Asset Folders"))
            repository.PopulateFromFolders();
    }
}
