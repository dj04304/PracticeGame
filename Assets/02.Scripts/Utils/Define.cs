public class Define
{
    public enum ObjectsType
    {
        Unknown,
        Player,
        NPC,
        Money,
        HandCroassant,
        ProjectileMoney,
    }

    public enum BuildObjectType
    {
        Sit,
        LockPlane,
    }

    public enum NextTutorial
    {
        Oven,
        Basket,
        CashTable,
        CashPoint,
        Trash,
        Unlock
    }

    public enum State
    {
        Moving,
        Idle,
        StackMoving,
        StackIdle,
        Sitting,
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
        Money = 13,
    }

    public enum ArriveType
    {
        CotainType,
        StackType,
        CashType,
        BagType,
        NomalType,
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

    public enum Particle
    {
        Loop,
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
