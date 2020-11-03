using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;


public struct Pos
{
    public int Y;
    public int X;

    public Pos(int y, int x) { Y = y; X = x; }
}

public class MapManager
{
    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    public int SizeX { get { return MaxX - MinX + 1; } }
    public int SizeY { get { return MaxY - MinY + 1; } }


    bool[,] m_collision;


    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        return !m_collision[y, x];
    }

    public void LoadMap(int mapId)
    {
        DestoryMap();

        string mapName = "Map_" + mapId.ToString("000");
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map"; 

        GameObject collision = Util.FindChild(go, "Collision", true);
        if (collision != null)
            collision.SetActive(false);

        CurrentGrid = go.GetComponent<Grid>();

        // Collision 관련 파일
        TextAsset txt = Managers.Resource.Load<TextAsset>($"Map/{mapName}");
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;

        m_collision = new bool[yCount, xCount];
        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                m_collision[y, x] = (line[x] == '1' ? true : false);
            }
        }
    }

    public void DestoryMap()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }


}
