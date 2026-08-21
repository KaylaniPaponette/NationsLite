using System;
using System.Reflection;
using UnityEngine;

public class SingletonAssetPathAttribute : Attribute
{
    public string assetName;

    public SingletonAssetPathAttribute(string assetName)
    {
        this.assetName = assetName;
    }
}

public class SingletonAsset<T> : ScriptableObject
    where T : ScriptableObject
{
    static T _instance;

    public static T instance
    {
        get 
        {
            if (!_instance)
                _instance = LoadFromAssetRepository();
            return _instance;
        }
    }

    static string assetName = typeof(T).GetCustomAttribute<SingletonAssetPathAttribute>()?.assetName;

    static T LoadFromAssetRepository()
    {
        return AssetManager.instance.LoadAsset<T>(assetName);
    }
}
