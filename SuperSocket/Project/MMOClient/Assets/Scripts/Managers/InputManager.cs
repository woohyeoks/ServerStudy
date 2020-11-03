using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager 
{
    // 일종에 델리게이트
    public Action KeyAction = null;

    // 이벤트 발생시 전체적으로 뿌린다 Listener 패턴
    public void OnUpdate()
    {
        if (Input.anyKey == false)
            return;

        if (KeyAction != null)
            KeyAction.Invoke();
    }

  
}
