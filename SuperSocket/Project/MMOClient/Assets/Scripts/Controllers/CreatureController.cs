using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    public float m_speed = 5.0f;
    protected Animator m_animator;
    protected MoveDir m_lastDir = MoveDir.Down;

    PositionInfo m_posInfo = new PositionInfo();

    public PositionInfo PosInfo
    {
        get { return m_posInfo; }
        set
        {
            if (m_posInfo.Equals(value))
                return;
            m_posInfo = value;
            UpdateAnimation();
        }
    }

    public Vector3Int CellPos
    {
        get {
            return new Vector3Int(PosInfo.posX, PosInfo.posY, 0);
        }
        set
        {
            var pos_info = new PositionInfo();
            pos_info = m_posInfo;
            pos_info.posX = value.x;
            pos_info.posY = value.y;
            PosInfo = pos_info;
        }
    }

    public MoveDir Dir
    {
        get { return PosInfo.moveDir; }
        set
        {
            if (PosInfo.moveDir == value)
                return;
            var pos_info = new PositionInfo();
            pos_info = m_posInfo;
            pos_info.moveDir = value;
            PosInfo = pos_info;
            if (value != MoveDir.None)
                m_lastDir = value;

            UpdateAnimation();
        }
    }


    public virtual CreatureState State
    {
        get { return PosInfo.state; }
        set 
        {
            if (PosInfo.state == value)
                return;
            var pos_info = new PositionInfo();
            pos_info = m_posInfo;
            pos_info.state = value;
            PosInfo = pos_info;
            UpdateAnimation();
        }
    }

    void Start()
    {
        Init();
    }


    protected virtual void Init()
    {
        m_speed = 5.0f;
        m_animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
    }

    // 스르륵 이동하는 것을 처리
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < m_speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * m_speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }


    protected virtual void MoveToNextPos()
    {
        if (Dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }

        Vector3Int destPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
            }
        }
    }

    protected virtual void UpdateAnimation()
    {
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }

        if (State == CreatureState.Idle)
        {
            switch (m_lastDir)
            {
                case MoveDir.Up:
                    m_animator.Play("idleUp");
                    break;
                case MoveDir.Down:
                    m_animator.Play("idleDown");
                    break;
                case MoveDir.Left:
                    m_animator.Play("idleLeft");
                    break;
                case MoveDir.Right:
                    m_animator.Play("idleRight");
                    break;
            }
        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    m_animator.Play("walkUp");
                    break;
                case MoveDir.Down:
                    m_animator.Play("walkDown");
                    break;
                case MoveDir.Left:
                    m_animator.Play("walkLeft");
                    break;
                case MoveDir.Right:
                    m_animator.Play("walkRight");
                    break;
            }
        }
    }
}
