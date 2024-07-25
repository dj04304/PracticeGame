using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpwningPool : SpawningPool
{
    private WaypointDatas _waypointDatas;

    private Queue<Transform> _breadWaypointQueue;

    // NPC ���� �̺�Ʈ�� �޾Ƽ� �����ִ� �뵵
    public void AddNPCCounter(int value) { _objCount += value; }
    // NPC �� �����ϴ� �ִ� ��
    public void SetKeepNPCCount(int count) { _keepObjCount = count; }

    // ���� ����Ʈ
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
            // ������ ũ�ξƻ� ���� ����
            npc.RanCroassantMaxCount = setCroassantCount;

            // BreadWaypoint �Ҵ�
            Transform breadWaypoint = null;
            if (_breadWaypointQueue.Count > 0)
            {
                breadWaypoint = _breadWaypointQueue.Dequeue();
                _breadWaypointQueue.Enqueue(breadWaypoint);
            }

            WaypointDatas npcWaypointsData = new WaypointDatas
            {
                FirstWaypoint = _waypointDatas.FirstWaypoint,
                BreadWaypoints = new List<Transform> { breadWaypoint }, // �� NPC���� �ϳ��� bread waypoint�� �Ҵ�
                CashTableWayPoint = _waypointDatas.CashTableWayPoint,
                CashTableWayPointToSit = _waypointDatas.CashTableWayPointToSit,
                SitTableWayPoint = _waypointDatas.SitTableWayPoint,
                EntranceWayPoint = _waypointDatas.EntranceWayPoint
            };

            npc.SetWaypointsData(npcWaypointsData);

            // ���� ���� ��ġ
            obj.transform.position = _spawnPos;
            obj.transform.SetParent(gameObject.transform);
            break;
        }

        _reserveCount--;
    }
}
