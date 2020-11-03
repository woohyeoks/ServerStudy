using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);


        base.Init();
        SceneType = Define.Scene.Game;
        Managers.Map.LoadMap(1);
        Screen.SetResolution(640, 480, false);
    }

    public override void Clear()
    {
    }
}
