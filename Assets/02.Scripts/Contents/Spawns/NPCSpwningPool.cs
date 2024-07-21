using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpwningPool : SpawningPool
{
    [SerializeField]
    private Transform _firstWaypoint;  // ù ��° ��ġ

    // target����Ʈ (�� �ٱ���)
    [SerializeField]
    private List<Transform> _targetWaypoints;

    private Queue<Transform> _waypointQueue;

    //NPC ���� �̺�Ʈ�� �޾Ƽ� �����ִ� �뵵
    public void AddNPCCounter(int value) { _objCount += value; }
    //NPC �� �����ϴ� �ִ� ��
    public void SetKeepNPCCount(int count) { _keepObjCount = count; }

    public void SetFirstWaypoint(Transform firstWaypoint){  _firstWaypoint = firstWaypoint; }

    //NPC�� �����ϴ� ��ġ�� List
    public void SetTargetWaypoints(List<Transform> targetWaypoints)
    {
        _targetWaypoints = targetWaypoints;
        _waypointQueue = new Queue<Transform>(_targetWaypoints);  // ť�� ��ȯ
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

            // ù ��° ��ġ�� ������ ��ǥ ��ġ ����
            Transform WayPoint = _waypointQueue.Dequeue(); // �� NPC�� ���� �ٸ��� ����
            npc.SetWaypoints(_firstWaypoint, WayPoint);

            _reserveCount--;
        }

    }
}
