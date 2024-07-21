using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroassantProjectile : BaseProjectile
{
    public override  void Initialize(Vector3 startPosition, Vector3 targetPosition)
    {
        base.Initialize(startPosition, targetPosition);
    }

    protected override Transform GetArrivalTarget() { return _playerController.BreadPosition; }
   
    protected override void Arrive()
    {
        base.Arrive();
    }
   
    // 크로아상 발사
    protected override void Update()
    {
       base.Update();
    }



}
