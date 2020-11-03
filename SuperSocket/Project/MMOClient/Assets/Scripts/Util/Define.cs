using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
    public struct PositionInfo
    {
        public string name;
        public int id;
        public CreatureState state;
        public MoveDir moveDir;
        public int posX;
        public int posY;
    }
    public enum CreatureState
    {
        Idle ,
        Moving ,
        Skill ,
        Dead
    }

    public enum MoveDir
    {
        None,
        Up,
        Down,
        Left , 
        Right
    }

    public enum Scene
    {
        Unknown ,
        Login , 
        Lobby , 
        Game ,
    }
}
