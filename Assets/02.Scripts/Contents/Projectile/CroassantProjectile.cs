using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CroassantProjectile : BaseProjectile
{
    public override  void Initialize(Vector3 startPosition, Vector3 targetPosition, Transform arrivalTarget, Define.ArriveType arriveType)
    {
        base.Initialize(startPosition, targetPosition, arrivalTarget, arriveType);
    }

    protected override Transform GetArrivalTarget() { return _arrivalTarget; }

    protected override void Arrive(ArriveType arriveType)
    {
        base.Arrive(arriveType);
    }

    // 크로아상 발사
    protected override void Update()
    {
       base.Update();
    }



}
