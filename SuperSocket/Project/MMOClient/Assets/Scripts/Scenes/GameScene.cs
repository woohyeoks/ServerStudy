using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class GameScene : BaseScene
{
    public enum MySockEventType
    {
        Connected,
        Disconnected,
        Recv,
    }
    public class MySockEvent
    {
        public MySockEventType Type;
        public string recvLine;
    }

    public GameObject panelConnected;
    public GameObject panelNeedConnect;
    public Text LogConnected;
    public Text LogConnected2;
    public InputField inputAddr;
    public InputField inputName;
    public Button buttonConnect;
    public Text LogNeedConnect;

    public static MySocket sock;
    private List<MySockEvent> sockEvents;
    private bool isPingWait = false;
    private bool isLogined = false;
    private bool isConnected = false;

    private float lastPing;
    private float lastLog;
    private int countPing;
    // 로그용
    private float log2ping = 0;
    private string log2ver = "0.1";
    private List<string> listLogInGame = new List<string>();
    private float LastLogInGame = 0;
    public static float inputX = 0;
    public static float inputY = 0;
    protected override void Init()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);


        base.Init();
        SceneType = Define.Scene.Game;

        Managers.Map.LoadMap(1);

        Screen.SetResolution(640, 480, false);


        panelConnected.SetActive(false);
        panelNeedConnect.SetActive(true);
        LogConnected.text = "";
        LogConnected2.text = "";

        sock = new MySocket();
        sockEvents = new List<MySockEvent>();
        inputAddr.text = PlayerPrefs.GetString("LastAddr", "");
        inputName.interactable = false;
        LogNeedConnect.text = "";

        // 데이터 받음, 유니티 기능 쓰지말 것(메인쓰레드 아님)
        List<MySockEvent> tempSocketRecvList = new List<MySockEvent>();

        buttonConnect.onClick.AddListener(delegate
        {
            // 연결 버튼 누를 때
            var arr = inputAddr.text.Split(':');
            if (arr.Length < 2)
            {
                LogNeedConnect.text = "address parsing fail, ex) 127.0.0.1 : 30001";
                return;
            }
            // 입력 주소 파싱
            var ip = arr[0].Trim();
            var portStr = arr[1].Trim();
            int port = 0;

            if (!int.TryParse(portStr, out port))
            {
                LogNeedConnect.text = "PORT value is number only, input: " + portStr;
                return;
            }

            PlayerPrefs.SetString("LastAddr", inputAddr.text);
            buttonConnect.interactable = false;

            LogNeedConnect.text = "try connect, " + ip + " : " + port;

            // 기존에 소켓은 날려버림
            if (sock != null)
            {
                sock.TryClose();
                sock = new MySocket();
            }

            // 쌓인 이벤트도 제거
            lock (sockEvents)
            {
                sockEvents.Clear();
            }

            sock.TryConnect(
               ip,
               port,
               SocketEvent_OnConnected,
               SocketEvent_OnRecv,
               SocketEvent_Disconnected);


        });

        // 소켓 연결됨 , 유니티 기능 쓰지말 것 (메인 쓰레드 아님)
        void SocketEvent_OnConnected()
        {
            lock (sockEvents)
            {
                var e = new MySockEvent();
                e.Type = MySockEventType.Connected;
                sockEvents.Add(e);
                Debug.Log("연결 성공");
            }
        }

        // 소켓 에러발생(끊김), 유니티 기능 쓰지말 것(메인쓰레드 아님)
        void SocketEvent_Disconnected(string err)
        {
            var e = new MySockEvent();
            e.Type = MySockEventType.Disconnected;
            e.recvLine = err;
            sockEvents.Add(e);
            Debug.Log("연결 끊김");
        }

       
        void SocketEvent_OnRecv(byte[] bin, int len)
        {
            Debug.Log("메세지 수신");
            for (int i = 0; i < len; i++)
            {
                if (bin[i] == (byte)'\r')
                {
                    var msg = System.Text.Encoding.UTF8.GetString(sock.Buf, 0, sock.BufPos);
                    sock.BufPos = 0;

                    var e = new MySockEvent();
                    e.Type = MySockEventType.Recv;
                    e.recvLine = msg;
                    tempSocketRecvList.Add(e);
                }
                else if (bin[i] == (byte)'\n')
                {
                    // skip
                }
                else
                {
                    sock.Buf[sock.BufPos++] = bin[i];
                }
            }

            // 이벤트를 한번 lock 에 많이 보내기 위해
            if (tempSocketRecvList.Count > 0)
            {
                lock (sockEvents)
                {
                    foreach (var e in tempSocketRecvList)
                    {
                        sockEvents.Add(e);
                    }
                }
                tempSocketRecvList.Clear();
            }

        }
    }


    private void Update()
    {
        if (sockEvents.Count > 0)
        {
            MySockEvent[] arr;
            lock (sockEvents)
            {
                arr = sockEvents.ToArray();
                sockEvents.Clear();
            }
            foreach (var e in arr)
            {
                if (e.Type == MySockEventType.Connected)
                {
                    panelConnected.SetActive(true);
                    LogConnected.text = "connected";
                    panelNeedConnect.SetActive(false);

                    isConnected = true;
                    var userName = inputName.text;
                    sock.SendPacket("login", userName);

                }
                else if (e.Type == MySockEventType.Disconnected)
                {
                    panelConnected.SetActive(false);
                    panelNeedConnect.SetActive(true);
                    LogNeedConnect.text = "disconnected";
                    buttonConnect.interactable = true;
                    isLogined = false;
                    isPingWait = false;


                    if (isConnected)
                    {
                        isConnected = false;
                        sock.TryClose();
                    }
                    //player.gameObject.SetActive(false);
                }
                else if (e.Type == MySockEventType.Recv)
                {
                    var cmds = e.recvLine.Split(',');
                    if (cmds[0] == "ping")
                    {
                        isPingWait = false;
                        countPing++;
                        if (countPing % 10 == 0)
                        {
                            log2ping = Time.realtimeSinceStartup - lastPing;
                        }
                        float.TryParse(cmds[1], out inputX);
                        float.TryParse(cmds[2], out inputY);
                    }
                    else if (cmds[0] == "update")
                    {
                        
                        var name2 = cmds[1];
                        var posInfo = new PositionInfo();
                        posInfo.id = int.Parse(cmds[2]);
                        posInfo.posX = int.Parse(cmds[3]);
                        posInfo.posY = int.Parse(cmds[4]);
                        posInfo.state = (CreatureState)int.Parse(cmds[5]);
                        posInfo.moveDir = (MoveDir)int.Parse(cmds[6]);


                        GameObject go = Managers.Object.FindById(posInfo.id);
                        if (go == null)
                        {
                            LogInGame(e.recvLine);
                            Managers.Object.AddPlayer(posInfo, false);
                            //go.SetActive(true);
                        }
                        else
                        {
                            CreatureController cc = go.GetComponent<CreatureController>();
                            if (cc != null)
                            {
                                cc.PosInfo = posInfo;
                            }
                        }


                        LogInGameSometime(e.recvLine);
                    }
                    else if (cmds[0] == "login")
                    {
                        var name2 = cmds[1];

                        var posInfo = new PositionInfo();
                        posInfo.name = name2;
                        posInfo.id = int.Parse(cmds[2]);
                        posInfo.posX = int.Parse(cmds[3]);
                        posInfo.posY = int.Parse(cmds[4]);
                        posInfo.state = (CreatureState)int.Parse(cmds[5]);
                        posInfo.moveDir = (MoveDir)int.Parse(cmds[6]);
                       
                        Managers.Object.AddPlayer(posInfo, isMe: true);

                        inputName.text = name2; // 마지막 일므 저장

                        isLogined = true;
                        LogNeedConnect.text = "login ok";

                        /* player.gameObject.SetActive(true);
                         player.transform.position = new Vector3(sx, sy, sz);*/
                    }
                    else if (cmds[0] == "logout")
                    {
                        LogInGame(e.recvLine);

                        var name2 = cmds[1];

                        // Managers.Object.Remove(name2);
                        LogInGame("logout, " + name2);
                        /*  if (others.ContainsKey(name2))
                          {
                              var o = others[name2];
                              others.Remove(name2);
                              GameObject.Destroy(o);

                              LogInGame("logout, " + name2);
                          }*/
                    }
                    else
                    {
                        Debug.LogError("recv unknown, " + e.recvLine);
                    }
                }
            }
        }

        // 로그인되었고 내가 핑응답을 받지 않았다면 핑하나 보낸다.
        // 동시에 2개의 핑이 존재하지 않도록
        if (isLogined && isPingWait == false)
        {
            isPingWait = true;
            lastPing = Time.realtimeSinceStartup;
            /*  // 주기적으로 입력값을 보낸다.
              float x = 0;
              float y = 0;
              if (Application.isFocused)
              {
                  x = Input.GetAxis("Horizontal");
                  y = Input.GetAxis("Vertical");
              }*/

            string name = "";
            int id = 0;
            int px = 0;
            int py = 0;
            CreatureState state = CreatureState.Idle;
            MoveDir dir = MoveDir.Down;
            if (Managers.Object.MyPlayer != null)
            {
                var info = Managers.Object.MyPlayer.PosInfo;
                name = info.name;
                id = info.id;
                px = info.posX;
                py = info.posY;
                state = info.state;
                dir = info.moveDir;

                Debug.Log($"x : {px} y ; {py}");
            }

            sock.SendPacket("ping", name, id, px, py, (int)state, (int)dir);
        }

        // 화면 로그 표시 갱신
        if (Time.realtimeSinceStartup - lastLog > 1.0f)
        {
            lastLog = Time.realtimeSinceStartup;

            var sb = new StringBuilder();
            int i = 0;
            foreach (var l in listLogInGame)
            {
                i++;
                if (i != 1)
                    sb.Append("\r\n");
                sb.Append(l);
            }
            LogConnected.text = sb.ToString();

            var log2 = string.Format("{0:0.0}ms / {1:0}fps\nv{2} / {3}\n{4}{5}{6}"
                , log2ping * 1000.0f
                , 1.0f / Time.deltaTime
                , log2ver
                , inputName.text
                , isLogined ? "L" : " "
                , isPingWait ? "P" : " "
                , Application.isFocused ? "F" : " "
            );
            LogConnected2.text = log2;
        }

    }

    // 게임중 로그를 표시
    void LogInGame(string msg)
    {
        listLogInGame.Add(msg);
        if (listLogInGame.Count > 4)
        {
            listLogInGame.RemoveAt(0);
        }
    }

    // 3초 마다 한번씩 찍는다.
    // 너무 자주오는 로그를 찍을때 사용
    void LogInGameSometime(string msg)
    {
        if (Time.realtimeSinceStartup - LastLogInGame < 3)
            return;
        LastLogInGame = Time.realtimeSinceStartup;
        LogInGame(msg);
    }

    public override void Clear()
    {

    }

}
