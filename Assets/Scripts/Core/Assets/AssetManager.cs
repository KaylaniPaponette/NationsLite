using System.Collections.Generic;
using UnityEngine;

public interface IAssetLoader
{
    public T LoadAsset<T>(string assetPath)
        where T: UnityEngine.Object;

    public T LoadAsset<T>(int id)
        where T : UnityEngine.Object;
        
    public List<T> LoadAssetsOfType<T>()
        where T : UnityEngine.Object;

    public List<T> LoadAssetsOfType<T>(string assetPath)
        where T : UnityEngine.Object;

    public void UnloadAsset<T>(string assetPath, T asset);
}

public class AssetManager : Singleton<AssetManager>
{
    IAssetLoader loader;
    const string kRepositoryAssetName = "AssetRepo";
    
    public void LoadRepository()
    {
        loader = Resources.Load<AssetRepository>(kRepositoryAssetName);
        if (loader == null)
            Debug.LogError("Failed to load asset repository");
    }


    public T LoadAsset<T>(string assetName)
        where T : UnityEngine.Object
    {
        return loader.LoadAsset<T>(assetName);
    }

    public T LoadAsset<T>(int id)
        where T : UnityEngine.Object
    {
        return loader.LoadAsset<T>(id);
    }

    public List<T> LoadAssetsOfType<T>()
        where T : UnityEngine.Object
    {
        return loader.LoadAssetsOfType<T>();
    }

    public List<T> LoadAssetsOfType<T>(string assetPath)
        where T : UnityEngine.Object
    {
        return loader.LoadAssetsOfType<T>(assetPath);
    }

    public void UnloadAsset<T>(string assetPath, T asset)
    {
        loader.UnloadAsset<T>(assetPath, asset);
    }
}