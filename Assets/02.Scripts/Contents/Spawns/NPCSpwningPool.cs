using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpwningPool : SpawningPool
{
    private WaypointDatas _waypointDatas;

    private Queue<Transform> _breadWaypointQueue;

    // NPC 수를 이벤트로 받아서 더해주는 용도
    public void AddNPCCounter(int value) { _objCount += value; }
    // NPC 를 유지하는 최대 수
    public void SetKeepNPCCount(int count) { _keepObjCount = count; }

    // 웨이 포인트
    public void SetWaypointsData(WaypointDatas waypointDatas)
    {
        _waypointDatas = waypointDatas;
        _spawnPos = waypointDatas.EntranceWayPoint.position;
        _breadWaypointQueue = new Queue<Transform>(waypointDatas.BreadWaypoints);
    }

    public override void Init()
    {
        PlayerController playerController = GameManager.Instance.Spawn.GetPlayer().GetComponent<PlayerController>();

        //_maxCount = playerController.CroassantMaxCount() - 5;

        GameManager.Instance.Spawn.OnNPCSpawnEvent -= AddNPCCounter;
        GameManager.Instance.Spawn.OnNPCSpawnEvent += AddNPCCounter;
    }

    void Update()
    {
        while (_reserveCount + _objCount < _keepObjCount)
        {
            StartCoroutine(ReserveSpawnCo());
        }
    }


    IEnumerator ReserveSpawnCo()
    {
        _reserveCount++;

        yield return new WaitForSeconds(Random.Range(0, _spawnTime));

        //int setCroassantCount = UnityEngine.Random.Range(_minCount, _maxCount);
        GameObject obj = GameManager.Instance.Spawn.Spawn(Define.ObjectsType.NPC, "NPC");
        NPCController npc = obj.GetComponent<NPCController>();

        int setCroassantCount;

        while (true)
        {
            if (_reserveCount == 1)
            {
                setCroassantCount = 1;
            }
            else
            {
                setCroassantCount = 2;
            }
            // 랜덤한 크로아상 개수 세팅
            npc.RanCroassantMaxCount = setCroassantCount;

            // BreadWaypoint 할당
            Transform breadWaypoint = null;
            if (_breadWaypointQueue.Count > 0)
            {
                breadWaypoint = _breadWaypointQueue.Dequeue();
                _breadWaypointQueue.Enqueue(breadWaypoint);
            }

            WaypointDatas npcWaypointsData = new WaypointDatas
            {
                FirstWaypoint = _waypointDatas.FirstWaypoint,
                BreadWaypoints = new List<Transform> { breadWaypoint }, // 각 NPC에게 하나의 bread waypoint만 할당
                CashTableWayPoint = _waypointDatas.CashTableWayPoint,
                CashTableWayPointToSit = _waypointDatas.CashTableWayPointToSit,
                SitTableWayPoint = _waypointDatas.SitTableWayPoint,
                EntranceWayPoint = _waypointDatas.EntranceWayPoint
            };

            npc.SetWaypointsData(npcWaypointsData);

            // 랜덤 스폰 위치
            obj.transform.position = _spawnPos;
            obj.transform.SetParent(gameObject.transform);
            break;
        }

        _reserveCount--;
    }
}
