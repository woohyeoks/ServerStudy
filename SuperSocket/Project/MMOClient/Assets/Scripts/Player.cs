using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Managers mg =  Managers.Instance;


        // 구독 신청
        Managers.Input.KeyAction -= OnKeyBoard;// 혹시 다른곳에서 구독신청 될 수 있어서
        Managers.Input.KeyAction += OnKeyBoard;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnKeyBoard()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A 입력");
        }
    }
}
