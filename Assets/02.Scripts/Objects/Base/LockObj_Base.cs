using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LockObj_Base : Obj_Base
{
    public enum LockStatus
    {
        Unlocked,
        Locked,
    }

    private int _cost;

    public LockStatus Status { get; protected set; }
    public int Cost { get { return _cost; } }

    public abstract bool CanUnlock(PlayerInfo player);

    public virtual void Unlock(PlayerInfo player)
    {
        if (CanUnlock(player))
        {
            Status = LockStatus.Unlocked;
            OnUnlock(player);
        }
    }

    public abstract override void Init();

    protected abstract void OnUnlock(PlayerInfo player);

    protected abstract Vector3 SetNextLockPos();
    protected abstract Quaternion SetNextLockRot();
    protected abstract Vector3 SetNextLockScale();
}
