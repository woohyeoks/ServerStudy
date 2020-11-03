using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers g_Instance;
    public static Managers Instance { get { Init();  return g_Instance; } }


    InputManager m_input = new InputManager();
    ResourcesManager m_resource = new ResourcesManager();
    ObjectManager m_object = new ObjectManager();
    MapManager m_map = new MapManager();
    public static InputManager Input { get { return Instance.m_input; } }
    public static ResourcesManager Resource { get { return Instance.m_resource; } } 
    public static ObjectManager Object { get { return Instance.m_object; } }
    public static MapManager Map { get { return Instance.m_map; } }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        m_input.OnUpdate();
    }
    
    private static void Init()
    {
        if (g_Instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            g_Instance = go.GetComponent<Managers>();
        }
    }
}
