using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetRepository : ScriptableObject, IAssetLoader
{
    public List<AssetGroup> assetGroups = new List<AssetGroup>();

    public T LoadAsset<T>(string assetName)
        where T : UnityEngine.Object
    {
        foreach (var group in assetGroups)
        {
            foreach (var assetEntry in group.assets)
            {
                if (assetEntry.key != assetName)
                    continue;

                var result = assetEntry.asset as T;
                if (result == null)
                    continue;
                return result;
            }
        }

        Debug.LogWarning($"Asset with key '{assetName}' not found in AssetRepository");
        return null;
    }

    public T LoadAsset<T>(int id)
        where T : UnityEngine.Object
    {
        foreach (var group in assetGroups)
        {
            foreach (var assetEntry in group.assets)
            {
                if (assetEntry.key != id.ToString())
                    continue;
                return assetEntry.asset as T;
            }
        }
        return null;
    }

    public List<T> LoadAssetsOfType<T>()
        where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        foreach (var group in assetGroups)
        {
            foreach (var assetEntry in group.assets)
            {
                var asset = assetEntry.asset as T;
                if (asset)
                    assets.Add(asset);
            }
        }

        return assets;
    }

    public List<T> LoadAssetsOfType<T>(string assetPath)
         where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        foreach (var group in assetGroups)
        {
            if (group.folderName != assetPath)
                continue;
            
            foreach (var entry in group.assets)
            {
                var asset = entry.asset as T;
                if (!asset)
                    continue;
                assets.Add(asset);
            }
        }
        return assets;
    }

    public Dictionary<string, T> LoadAssetsAsDictionary<T>()
        where T : UnityEngine.Object
    {
        Dictionary<string, T> assets = new Dictionary<string, T>();
        foreach (var group in assetGroups)
        {
            foreach (var assetEntry in group.assets)
            {
                var asset = assetEntry.asset as T;
                if (asset)
                    assets.Add(assetEntry.key, asset);
            }
        }
        return assets;
    }

    public void UnloadAsset<T>(string assetPath, T asset)
    {
        throw new System.NotImplementedException();
    }

#if UNITY_EDITOR
    public void PopulateFromFolders()
    {
        foreach (var group in assetGroups)
            group.assets.Clear();
    
        foreach (var group in assetGroups)
        {
            string folderPath = group.folderName.TrimEnd('/');
            string[] assetGUIDs = AssetDatabase.FindAssets("", new[] { folderPath });
            foreach (var guid in assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.StartsWith(folderPath + "/"))
                    continue;

                var relativePath = path.Substring(folderPath.Length + 1);
                if (relativePath.Contains("/"))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

                if (asset == null)
                    continue;
                if (asset is DefaultAsset)
                    continue;

                var entry = new AssetEntry();
                entry.asset = asset;
                if (group.useNumberedKey)
                    entry.key = group.assets.Count.ToString();
                else
                    entry.key = asset.name;

                group.assets.Add(entry);
            }
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}