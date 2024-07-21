using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager
{
    GameObject _player;
    //Dictionary<int, GameObject> _playerDic = new Dictionary<int, GameObject>();
    HashSet<GameObject> _npc = new HashSet<GameObject>();
    HashSet<GameObject> _handCroassant = new HashSet<GameObject>();

    // �����ؼ� npc Spawn�ϴ� �뵵
    public Action<int> OnSpawnEvent;

    public GameObject GetPlayer() { return _player; }

    // Spawn�� ĳ������ ID, ������Ʈ�� ������ �ȴ�.
    public GameObject Spawn(Define.ObjectsType type, string path, Transform parent = null)
    {
        GameObject go = GameManager.Instance.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.ObjectsType.NPC:
                _npc.Add(go);
                OnSpawnEvent?.Invoke(1);
                break;
            case Define.ObjectsType.Player:
                _player = go;
                break;
            case Define.ObjectsType.HandCroassant:
                _handCroassant.Add(go);
                break;
        }

        return go;
    }

    public Define.ObjectsType GetObjectsType(GameObject go)
    {
        // PlayerState �� MonsterState���� type�� ������ �� ����
        //BaseState bc = go.GetComponent<BaseState>();

        //if (bc == null)
        //    return Define.ObjectsType.Unknown;


        return Define.ObjectsType.NPC;
    }

    // Despawn NPC ���� �� ����
    public void Despawn(GameObject go)
    {
        Define.ObjectsType type = GetObjectsType(go);

        switch (type)
        {
            case Define.ObjectsType.NPC:
                {
                    if (_npc.Contains(go))
                    {
                        _npc.Remove(go);
                        OnSpawnEvent?.Invoke(-1);
                    }
                }
                break;
            case Define.ObjectsType.Player:
                {
                    if (_player == go)
                        _player = null;
                }
                break;
        }

        GameManager.Instance.Resource.Destroy(go);
    }
}
