using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }
        return Object.Instantiate(prefab, parent); // Object 안붙히면 재귀적으로 호출하니깐
    }

    public void Destroy(GameObject go, float delay = 3.0f)
    {
        if (go == null)
            return;
        Object.Destroy(go, delay);
    }
}
