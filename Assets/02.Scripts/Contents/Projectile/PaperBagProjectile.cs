using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PaperBagProjectile : BaseProjectile
{
    public override void Initialize(Vector3 startPosition, Vector3 targetPosition, Transform arrivalTarget, Define.ArriveType arriveType, int stackCount = 0, float duration = 0.1f)
    {
        base.Initialize(startPosition, targetPosition, arrivalTarget, arriveType, stackCount, duration);
    }

    protected override Transform GetArrivalTarget() { return _arrivalTarget; }

    protected override void Arrive(ArriveType arriveType)
    {
        base.Arrive(arriveType);
    }
}
