using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class WaypointDatas
{
    public Transform FirstWaypoint;
    public List<Transform> BreadWaypoints;
    public Transform CashTableWayPoint;
    public Transform CashTableWayPointToSit;
    public Transform SitTableWayPoint;
    public Transform EntranceWayPoint;
}


public class GameScene : BaseScene
{
    [SerializeField]
    WaypointDatas waypointDatas;

    [Header("Other Transform")]
    [SerializeField]
    private Transform _breadSpawnPoint;

    [SerializeField]
    private Transform _playerStartPoint;

    [SerializeField]
    int _npcSpawnCount = 4;

    // 시작시 바로 실행
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game; 

        GameObject player = GameManager.Instance.Spawn.Spawn(Define.ObjectsType.Player, "Chef");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPalyer(player);
        player.gameObject.transform.position = _playerStartPoint.position;

        GameObject go =  new GameObject { name = "@SpawningPool" };
        NPCSpwningPool pool = go.GetOrAddComponent<NPCSpwningPool>();

        GameManager.Instance.Object.ShowObj<Sit>();

        //pool.SetSpawnPoint(_spawnPoint);
        //pool.SetFirstWaypoint(_firstWaypoint);
        //pool.SetBreadWaypoints(_breadWaypoints);

        pool.SetWaypointsData(waypointDatas);

        // NPC 생성 Count
        pool.SetKeepNPCCount(_npcSpawnCount);



        
    }

    public override void Clear()
    {
        
    }


}
