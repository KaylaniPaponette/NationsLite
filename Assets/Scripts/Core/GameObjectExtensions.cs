using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;


#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public static class GameObjectExtensions
{
    public static void SafeDestroy(this GameObject obj)
    {
        if (obj)
            GameObject.Destroy(obj);
    }

    public static void SafeDestroyGameObject(this Component component)
    {
        if (component)
            GameObject.Destroy(component.gameObject);
    }

    /// <summary>
    /// Destroy any Unity object if it is not null or missing.
    /// </summary>
    public static void SafeDestroyImmediate(this Object obj, bool allowDestroyingAssets = false)
    {
        if (obj)
            Object.DestroyImmediate(obj, allowDestroyingAssets);
    }

    /// <summary>
    /// Destroy a component's GameObject if it is not null or missing.
    /// </summary>
    public static void SafeDestroyGameObjectImmediate(this Component component)
    {
        if (component)
            GameObject.DestroyImmediate(component.gameObject);
    }

    public static void DestroyChildren(this GameObject gameObject)
    {
        var transform = gameObject.transform;
        for (int i = transform.childCount - 1; i >= 0; --i)
            Object.Destroy(transform.GetChild(i).gameObject);
    }

    public static void DestroyChildrenImmediate(this GameObject gameObject)
    {
        var transform = gameObject.transform;
        for (int i = transform.childCount - 1; i >= 0; --i)
            Object.DestroyImmediate(transform.GetChild(i).gameObject);
    }

    /// <summary>
    /// Instantiate a prefab, optionally without activating it.
    /// In editor, creates a live prefab link so that changes to the prefab will be reflected
    /// in the instance immediately.
    /// </summary>
    public static GameObject InstantiatePrefab(this GameObject gameObject, bool active = true)
    {
        if (!active)
            gameObject.SetActive(false);

        GameObject instance;
        try
        {
#if UNITY_EDITOR
            instance = (GameObject)PrefabUtility.InstantiatePrefab(gameObject);
            if (instance)
                return instance;
#endif
            instance = Object.Instantiate(gameObject);
        }
        finally
        {
            gameObject.SetActive(true);
        }
        return instance;
    }

    /// <summary>
    /// Instantiate a prefab, optionally without activating it.
    /// In editor, creates a live prefab link so that changes to the prefab will be reflected
    /// in the instance immediately.
    /// </summary>
    public static T InstantiatePrefab<T>(this T component, bool active = true)
        where T : Component
    {
        if (!active)
            component.gameObject.SetActive(false);

        T instance;
        try
        {
#if UNITY_EDITOR
            instance = (T)PrefabUtility.InstantiatePrefab(component);
            if (instance)
                return instance;
#endif
            instance = Object.Instantiate(component);
        }
        finally
        {
            component.gameObject.SetActive(true);
        }
        return instance;
    }

    public static int SafeCompareTo(this Object a, Object b)
    {
        string nameA = a ? a.name : "";
        string nameB = b ? b.name : "";
        return nameA.CompareTo(nameB);
    }

    public static bool ContainsAncestor(this GameObject @this, Transform ancestor)
    {
        return @this.transform.IsChildOf(ancestor);
    }

    public static void SetLayerRecursively(this GameObject @this, int layer)
    {
        @this.layer = layer;
        foreach (Transform child in @this.transform)
            child.gameObject.SetLayerRecursively(layer);
    }

    public static void SetLayerRecursively(this GameObject @this, int layer, uint renderingLayerMask)
    {
        @this.layer = layer;
        if (@this.TryGetComponent<Renderer>(out var renderer))
            renderer.renderingLayerMask = renderingLayerMask;
        foreach (Transform child in @this.transform)
            child.gameObject.SetLayerRecursively(layer, renderingLayerMask);
    }

    public static GameObject FindChildWithLayer(this GameObject @this, int layer)
    {
        foreach (Transform child in @this.transform)
        {
            if (child.gameObject.layer == layer)
                return child.gameObject;

            var found = child.gameObject.FindChildWithLayer(layer);
            if (found)
                return found;
        }
        return null;
    }

    public static GameObject FindChildWithTag(this GameObject @this, string tag)
    {
        foreach (Transform child in @this.transform)
        {
            if (child.CompareTag(tag))
                return child.gameObject;

            var found = child.gameObject.FindChildWithTag(tag);
            if (found)
                return found;
        }
        return null;
    }
}
