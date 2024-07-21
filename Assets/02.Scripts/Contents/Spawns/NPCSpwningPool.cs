using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpwningPool : SpawningPool
{
    [SerializeField]
    private Transform _firstWaypoint;  // 첫 번째 위치

    // target포인트 (빵 바구니)
    [SerializeField]
    private List<Transform> _targetWaypoints;

    private Queue<Transform> _waypointQueue;

    //NPC 수를 이벤트로 받아서 더해주는 용도
    public void AddNPCCounter(int value) { _objCount += value; }
    //NPC 를 유지하는 최대 수
    public void SetKeepNPCCount(int count) { _keepObjCount = count; }

    public void SetFirstWaypoint(Transform firstWaypoint){  _firstWaypoint = firstWaypoint; }

    //NPC가 가야하는 위치의 List
    public void SetTargetWaypoints(List<Transform> targetWaypoints)
    {
        _targetWaypoints = targetWaypoints;
        _waypointQueue = new Queue<Transform>(_targetWaypoints);  // 큐로 변환
    }

    public override void Init()
    {
        _spawnPos = new Vector3(-3f, 0.5f, 15f);

        GameManager.Instance.Spawn.OnSpawnEvent -= AddNPCCounter;
        GameManager.Instance.Spawn.OnSpawnEvent += AddNPCCounter;
        
    }


    // Update is called once per frame
    void Update()
    {
        while (_reserveCount + _objCount < _keepObjCount)
        {
            _reserveCount++;

            GameObject obj = GameManager.Instance.Spawn.Spawn(Define.ObjectsType.NPC, "NPC");

            NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();
            NPCController npc = obj.GetComponent<NPCController>();

            obj.transform.position = _spawnPos;
            obj.transform.SetParent(gameObject.transform);

            // 첫 번째 위치와 각자의 목표 위치 설정
            Transform WayPoint = _waypointQueue.Dequeue(); // 각 NPC에 대해 다르게 설정
            npc.SetWaypoints(_firstWaypoint, WayPoint);

            _reserveCount--;
        }

    }
}
