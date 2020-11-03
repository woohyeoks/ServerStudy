using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        // 나중에 네트워크를 통해 다른 플레이어들 조종 할 수 있어야한다.
        base.UpdateController();
    }

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }
    }

   

   
}
