using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SpawningPool : MonoBehaviour
{
    [SerializeField]
    protected int _objCount = 0;

    //에약된 카운트
    protected int _reserveCount = 0;

    // 유지하는 npc 개체 수
    [SerializeField]
    protected int _keepObjCount = 0;

    [SerializeField]
    protected Vector3 _spawnPos;

    // 주기적으로 나올 시간
    [SerializeField]
    protected float _spawnTime = 2.0f;

    void Start()
    {
        Init();
    }

    public abstract void Init();

}
