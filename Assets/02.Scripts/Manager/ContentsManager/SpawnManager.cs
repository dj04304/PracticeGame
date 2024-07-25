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
    HashSet<GameObject> _money = new HashSet<GameObject>();

    // 구독해서 npc Spawn하는 용도
    public Action<int> OnNPCSpawnEvent;
    public Action<int> OnMoneySpawnEvent;

    public GameObject GetPlayer() { return _player; }

    // Spawn시 캐릭터의 ID, 오브젝트를 넣으면 된다.
    public GameObject Spawn(Define.ObjectsType type, string path, Transform parent = null)
    {
        GameObject go = GameManager.Instance.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.ObjectsType.NPC:
                _npc.Add(go);
                OnNPCSpawnEvent?.Invoke(1);
                break;
            case Define.ObjectsType.Player:
                _player = go;
                break;
            case Define.ObjectsType.HandCroassant:
                _handCroassant.Add(go);
                break;
            case Define.ObjectsType.ProjectileMoney:
                OnMoneySpawnEvent?.Invoke(1);
                _money.Add(go);
                break;
        }

        return go;
    }


    public Define.ObjectsType GetObjectsType(GameObject go)
    {
        return Define.ObjectsType.NPC;
    }

    // Despawn NPC 퇴장 시 삭제
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
                        OnNPCSpawnEvent?.Invoke(-1);
                    }
                }
                break;
            case Define.ObjectsType.Player:
                {
                    if (_player == go)
                        _player = null;
                }
                break;
            case Define.ObjectsType.Money:
                {
                    _money.Remove(go);
                    OnMoneySpawnEvent?.Invoke(-1);
                }
                break;
        }

        GameManager.Instance.Resource.Destroy(go);
    }
}
