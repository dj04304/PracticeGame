using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{

    #region AI Pos
    [SerializeField]
    private Transform _firstWaypoint;

    [SerializeField]
    private Transform _cashTableWayPoint;

    [SerializeField]
    private List<Transform> _breadWaypoints;
    #endregion

    [SerializeField]
    private Transform _breadSpawnPoint;

    [SerializeField]
    private Transform _playerStartPoint;

    [SerializeField]
    int _croassantCount = 50;

    [SerializeField]
    int _npcSpawnCount = 4;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game; 

        GameObject player = GameManager.Instance.Spawn.Spawn(Define.ObjectsType.Player, "Chef");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPalyer(player);

        player.gameObject.transform.position = _playerStartPoint.position;

        GameObject go =  new GameObject { name = "@SpawningPool" };
        NPCSpwningPool pool = go.GetOrAddComponent<NPCSpwningPool>();

        pool.SetFirstWaypoint(_firstWaypoint);
        pool.SetTargetWaypoints(_breadWaypoints);

        // NPC »ý¼º Count
        pool.SetKeepNPCCount(_npcSpawnCount);



        
    }

    public override void Clear()
    {
        
    }


}
