using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

// 라이브 중에 들어가면 안됨
// 컴파일러에게 알려주면 개발하는 단계에서는 사용 가능
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor
{
#if UNITY_EDITOR

    // % (Ctrl), #(Shift) , & (Alt)
    // 팝업을 떠서 새로운 오브젝트 만드는 함수
    [MenuItem("Tools/Sample")]
    private static void HelloWorld()
    {
        if (EditorUtility.DisplayDialog("Hellow", "Create?", "Create", "Cancel"))
        {
            new GameObject("Hello World");
        }
    }

    [MenuItem("Tools/GenerateMap %#g")]
    private static void GenerateMap()
    {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject go in gameObjects)
        {
            Tilemap tm = Util.FindChild<Tilemap>(go, "Collision", true);

            // 바이너리 형태는 0101 숫자를 1비트 단위로 압축해서 용량을 절약하는 방식
            // 텍스트 형태  문자로 관리하는 방식
            using (var writer = File.CreateText($"Assets/Resources/Map/{go.name}.txt"))
            {
                writer.WriteLine(tm.cellBounds.xMin);
                writer.WriteLine(tm.cellBounds.xMax);
                writer.WriteLine(tm.cellBounds.yMin);
                writer.WriteLine(tm.cellBounds.yMax);

                for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
                {
                    for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
                    {
                        // 타일 존재 여부 확인
                        TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null)
                        {
                            writer.Write("1");
                        }
                        else
                        {
                            writer.Write("0");
                        }
                    }
                    writer.WriteLine();
                }

                Debug.Log($"Write File : {go.name}");
            }
        }
       
       
    }


#endif

}
