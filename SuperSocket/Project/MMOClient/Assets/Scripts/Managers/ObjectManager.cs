using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> m_objects = new Dictionary<int, GameObject>();

    //List<GameObject> m_objects = new List<GameObject>();

    public void AddPlayer(PositionInfo info, bool isMe = false)
    {
        if (isMe)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
            go.name = info.name;
            m_objects.Add(info.id, go);

            MyPlayer = go.GetComponent<MyPlayerController>();
            MyPlayer.PosInfo = info;
        }
        else
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Player");
            go.name = info.name;
            m_objects.Add(info.id, go);

            PlayerController pc = go.GetComponent<PlayerController>();
            pc.PosInfo = info;
        }
    }


    public void Remove(int id)
    {
        m_objects.Remove(id);
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach (GameObject obj in m_objects.Values)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();

            if (cc == null)
                continue;

           /* if (cc.CellPos = cellPos)
                return obj;*/

        }
        return null;
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        m_objects.TryGetValue(id, out go);
        return go;
    }

    public void Clear()
    {
        m_objects.Clear();
    }

}
