using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SpawningPool : MonoBehaviour
{
    [SerializeField]
    protected int _objCount = 0;

    //����� ī��Ʈ
    protected int _reserveCount = 0;

    // �����ϴ� npc ��ü ��
    [SerializeField]
    protected int _keepObjCount = 0;

    [SerializeField]
    protected Vector3 _spawnPos;

    // �ֱ������� ���� �ð�
    [SerializeField]
    protected float _spawnTime = 2.0f;

    void Start()
    {
        Init();
    }

    public abstract void Init();

}
