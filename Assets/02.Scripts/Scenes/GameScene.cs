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

[Serializable]
public class TutorailPointDatas
{
    public Transform OvenPoint;
    public Transform BasketPoint;
    public Transform CashTablePoint;
    public Transform CashPoint;
    public List<Transform> UnLockPoint;
    public Transform TrashPoint;
}

public class GameScene : BaseScene
{
    [SerializeField]
    WaypointDatas waypointDatas;
    
    [SerializeField]
    TutorailPointDatas tutorailPointDatas;

    [Header("Other Transform")]
    [SerializeField]
    private Transform _breadSpawnPoint;

    [SerializeField]
    private Transform _playerStartPoint;

    [SerializeField]
    int _npcSpawnCount = 3;

    public GameObject SpawnRoot;

    // 시작시 바로 실행
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game; 

        GameObject player = GameManager.Instance.Spawn.Spawn(Define.ObjectsType.Player, "Chef");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPalyer(player);
        player.gameObject.transform.position = _playerStartPoint.position;

        SpawnRoot =  new GameObject { name = "@SpawningPool" };
        NPCSpwningPool pool = SpawnRoot.GetOrAddComponent<NPCSpwningPool>();

        GameManager.Instance.Tutorial.SetTutorialPoint(tutorailPointDatas);

        GameManager.Instance.Object.ShowObj<Sit>();
        GameManager.Instance.Object.ShowObj<LockPlane>();

        pool.SetWaypointsData(waypointDatas);

        // NPC 생성 Count
        pool.SetKeepNPCCount(_npcSpawnCount);

    }

    public override void Clear()
    {
        
    }


}
