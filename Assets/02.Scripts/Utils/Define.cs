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
        Player = 7,
        NPC = 8,
        Basket = 9, 
        CashTable = 10,
        BreadMachine = 11,
    }

    public enum ArriveType
    {
        Basket,
        BreadMachine,
        Default
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
