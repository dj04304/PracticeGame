using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum ObjectsType
    {
        Unknown,
        Player,
        NPC,
        Money,
        HandCroassant,
    }
    public enum State
    {
        Moving,
        Idle,
        StackMoving,
        StackIdle,
    }

    public enum Layer
    {
        Ground = 6,
        Block = 7,
        Player = 8,
        NPC = 9,
        Basket = 10, 
        CashTable = 11,
        BreadMachine = 12,
    }

    public enum ArriveType
    {
        NomalType,
        StackType,
        CashType,
        BagType,

    }

    public enum Scene
    {
        Unkown,
        Login,
        Game,

    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
    }

    public enum TouchEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
        Drag,
        DragEnd,
    }

    public enum CameraMode
    {
        QuaterView,
    }
}
