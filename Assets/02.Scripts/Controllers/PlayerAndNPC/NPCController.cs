using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class NPCController : BaseController
{
    [SerializeField]
    private Transform _handPos;

    private Transform _firstWaypoint;
    private Transform _breadWaypoint; // 각 NPC에게 할당된 하나의 BreadWaypoint
    private Transform _cashTableWayPoint;
    private Transform _cashTableWayPointToSit;
    private Transform _sitTableWayPoint;
    private Transform _entranceWayPoint;

    [SerializeField]
    private NPCState _currentState;

    private Vector3 _currentDestination;

    private bool _moving = false;
    private bool _hasSeenTarget = false; // 이미 타겟을 본 상태인지 저장

    private int _mask = (1 << (int)Define.Layer.Basket | (1 << (int)Define.Layer.CashTable));
    private int _playerNpcMask = (1 << (int)Define.Layer.Player | (1 << (int)Define.Layer.NPC));

    private bool IsInMask(int layer) { return (_mask & (1 << layer)) != 0; }

    private Quaternion _lastKnownRotation; // 마지막으로 바라본 회전값 저장

    public Transform HandPos => _handPos;

    private Stack<GameObject> _croassantStack = new Stack<GameObject>();
    private Stack<GameObject> _paperStack = new Stack<GameObject>();

    public static List<NPCController> _oddNPCList = new List<NPCController>();
    public static List<NPCController> _evenNPCList = new List<NPCController>();

    private readonly float _lineSpacingZ = 1.0f; // NPC 간 간격

    private enum NPCRequire
    {
        Sell,
        Sit,
    }

    private enum NPCState
    {
        Idle,
        MovingToFirstWaypoint,
        MovingToBreadWaypoint,
        MovingToCashTable,
        MovingToCashTableToSit,
        MovingToSitTable,
        MovingToEntrance
    }

    //public int TableToSit 
    //{
    //    get { return _tableToSit; }
    //    set { _tableToSit = value; }
    //}
    //public int TableToCash
    //{
    //    get { return _tableToCash; }
    //    set { _tableToCash = value; }
    //}

    // 랜덤한 크로아상의 개수 설정
    public int RanCroassantMaxCount { get; set; }
    public int GetCroassantStackCount() { return _croassantStack.Count; }

    UI_Balloon _ui;

    public void SetWaypointsData(WaypointDatas waypointsData)
    {
        _firstWaypoint = waypointsData.FirstWaypoint;
        // 할당된 하나의 BreadWaypoint
        _breadWaypoint = waypointsData.BreadWaypoints.Count > 0 ? waypointsData.BreadWaypoints[0] : null;
        _cashTableWayPoint = waypointsData.CashTableWayPoint;
        _cashTableWayPointToSit = waypointsData.CashTableWayPointToSit;
        _sitTableWayPoint = waypointsData.SitTableWayPoint;
        _entranceWayPoint = waypointsData.EntranceWayPoint;

        _currentState = NPCState.MovingToFirstWaypoint;
        _ui = GameManager.Instance.UI.MakeWorldSpaceUI<UI_Balloon>(transform);

        StartMovingToFirstWaypoint();
    }

    public override void Init()
    {
        MoveSpeed = 5.0f;
        WorldObjectType = Define.ObjectsType.NPC;
    }

    #region IDLE
    protected override void UpdateIdle()
    {
        if (!_hasSeenTarget) // 타겟을 본 적이 없으면
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.0f, _mask);

            if (hitColliders.Length > 0)
            {
                Collider closestCollider = null;
                float closestDistance = float.MaxValue;

                foreach (var collider in hitColliders)
                {
                    if (IsInMask(collider.gameObject.layer))
                    {
                        float distance = Vector3.Distance(transform.position, collider.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestCollider = collider;
                        }
                    }
                }

                if (closestCollider != null)
                {
                    Debug.Log($"Closest Collider: {closestCollider.gameObject.layer}"); // 디버그 로그

                    // 가장 가까운 사물을 바라보게 함
                    Vector3 direction = (closestCollider.transform.position - transform.position).normalized;

                    // 회전 계산
                    _lastKnownRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = _lastKnownRotation;

                    // UI 업데이트
                    if (closestCollider.gameObject.layer == (int)Define.Layer.Basket)
                        _ui.SetUIActive(true, bread: true, pay: false, chair : false);

                    _hasSeenTarget = true; // 타겟을 본 상태로 변경
                }
            }
        }
        else
        {
            // 타겟을 본 후에는 계속 같은 방향 유지
            transform.rotation = _lastKnownRotation;
        }
    }
    #endregion

    protected override void UpdateStackIdle()
    {
        if (!_hasSeenTarget) // 타겟을 본 적이 없으면
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10.0f, _mask);

            if (hitColliders.Length > 0)
            {
                Collider closestCollider = null;
                float closestDistance = float.MaxValue;

                foreach (var collider in hitColliders)
                {
                    if (IsInMask(collider.gameObject.layer))
                    {
                        float distance = Vector3.Distance(transform.position, collider.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestCollider = collider;
                        }
                    }
                }

                if (closestCollider != null)
                {
                    Debug.Log($"Closest Collider: {closestCollider.gameObject.layer}"); // 디버그 로그

                    // 가장 가까운 사물을 바라보게 함
                    Vector3 direction = (closestCollider.transform.position - transform.position).normalized;

                    // 회전 계산
                    _lastKnownRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = _lastKnownRotation;

                    // UI 업데이트
                    if (closestCollider.gameObject.layer == (int)Define.Layer.Basket)
                        _ui.SetUIActive(true, bread: true, pay: false, chair: false);

                    _hasSeenTarget = true; // 타겟을 본 상태로 변경
                }
            }
        }
        else
        {
            // 타겟을 본 후에는 계속 같은 방향 유지
            transform.rotation = _lastKnownRotation;
        }
    }

    // 일반적으로 움직일 경우는 빵 가지러갈때, 집에 갈때
    protected override void UpdateMoving()
    {
        switch (_currentState)
        {
            case NPCState.MovingToFirstWaypoint:
                {
                    Move(_firstWaypoint.position);
                    if (HasReachedDestination(_firstWaypoint.position))
                    {
                        _currentState = NPCState.MovingToBreadWaypoint;
                        SelectAtFirstWayPoint();
                    }
                }
                break;

            case NPCState.MovingToBreadWaypoint:
                {
                    Move(_breadWaypoint.position);
                    if (HasReachedDestination(_breadWaypoint.position))
                    {
                        State = Define.State.Idle;
                        _moving = false;
                    }
                }
                break;
            case NPCState.MovingToEntrance:
                {
                    Move(_entranceWayPoint.position);
                    _ui.SetUIActive(false, bread: false, pay: false, chair: false);
                    if (HasReachedDestination(_entranceWayPoint.position))
                    {
                        RemoveList();
                        GameManager.Instance.Spawn.Despawn(gameObject);
                    }
                }
                break;
        }
    }

    // stack moving => 빵이 손에 다찼을때, 캐쉬테이블 혹은 싯 테이블로 이동할 경우
    protected override void UpdateStackMoving()
    {
        switch (_currentState)
        {
            case NPCState.MovingToFirstWaypoint:
                {
                    Move(_firstWaypoint.position);
                    if (HasReachedDestination(_firstWaypoint.position))
                    {
                            if (RanCroassantMaxCount % 2 == 0)
                                _currentState = NPCState.MovingToCashTable;
                            else
                                _currentState = NPCState.MovingToCashTableToSit;
                        //PerformActionAtFirstWaypoint();
                    }
                }
                break;
            case NPCState.MovingToCashTable:
                    HandleMovingToCashTable();
                break;
            case NPCState.MovingToCashTableToSit:
                    HandleMovingToCashTableToSit();
                break;
            case NPCState.MovingToSitTable:
                {
                    Move(_sitTableWayPoint.position);
                    if (HasReachedDestination(_sitTableWayPoint.position))
                    {
                        State = Define.State.Idle;
                        _moving = false;
                    }
                }
                break;

            case NPCState.MovingToEntrance:
                {
                    Move(_entranceWayPoint.position);
                    _ui.SetUIActive(false, bread: false, pay: false, chair: false);
                    if (HasReachedDestination(_entranceWayPoint.position))
                    {
                        RemoveList();
                        RemoveToPaperBagStack();
                        GameManager.Instance.Spawn.Despawn(gameObject);
                    }
                }
                break;
        }
    }

    private void Move(Vector3 destination)
    {
        //Debug.Log($"{_npcList.Count} : {destination}");
        if (_moving)
        {
            if (!HasReachedDestination(destination))
            {
                // 타겟을 바라보는 변수 초기화
                _hasSeenTarget = false;

                Vector3 direction = (destination - transform.position).normalized;

                // 장애물 회피
                Vector3 avoidDirection = Vector3.zero;
                float avoidDistance = 2.0f;  // 장애물 회피 거리
                Collider[] colliders = Physics.OverlapSphere(transform.position, avoidDistance, ~_playerNpcMask);

                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject != gameObject)  // 자기 자신 제외
                    {
                        Vector3 awayFromObstacle = transform.position - collider.transform.position;
                        avoidDirection += awayFromObstacle.normalized / awayFromObstacle.magnitude;
                    }
                }

                if (avoidDirection != Vector3.zero)
                {
                    direction += avoidDirection * 0.5f;  // 회피 강도 조절
                    direction = direction.normalized;
                }

                if (direction != Vector3.zero)
                {
                    // 이동 방향과 회피 방향을 고려하여 NPC를 이동시킵니다.
                    direction = new Vector3(direction.x, 0, direction.z).normalized; // Y축은 0으로 고정
                    transform.position += direction * MoveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), MoveSpeed * Time.deltaTime);
                }
            }
        }
    }

    // 도착 여부
    private bool HasReachedDestination(Vector3 destination)
    {
        return Vector3.Distance(transform.position, destination) < 0.1f;
    }

    private void StartMovingToFirstWaypoint()
    {
        if (_firstWaypoint != null)
        {
            _currentDestination = _firstWaypoint.position;
            _moving = true;
            _currentState = NPCState.MovingToFirstWaypoint;

           if(_croassantStack.Count == RanCroassantMaxCount)
            {
                State = Define.State.StackMoving;
            }
           else
            {
                State = Define.State.Moving;

            }
        }
    }

    // firstwaypoint
    // -> 
    // 1. breadpoint
    // 2. cashtable
    public void SelectAtFirstWayPoint()
    {
        // 이후 BreadWaypoint로 이동 시작
        if(_croassantStack.Count == RanCroassantMaxCount) 
        {
            //_currentState = NPCState.MovingToCashTable;
            //StartMovingToCashTableWaypoint();
            StartMovingToFirstWaypoint();
        }
        else
        {
            _currentState = NPCState.MovingToBreadWaypoint;
            StartMovingToBreadWaypoint();
        }
       
    }

    // 집에 가기 위한 코드
    public void StartMovingToEntrance()
    {
        if (_entranceWayPoint != null)
        {
            _currentDestination = _entranceWayPoint.position;
            _currentState = NPCState.MovingToEntrance;
            State = Define.State.StackMoving;
            _moving = true;
        }
            
    }

    public void StartMovingSitPoint()
    {
        if(_sitTableWayPoint != null)
        {
            _currentDestination = _sitTableWayPoint.position;
            _currentState = NPCState.MovingToSitTable;
            _moving = true;
        }
    }

    private void StartMovingToBreadWaypoint()
    {
        if (_breadWaypoint != null)
        {
            _currentDestination = _breadWaypoint.position;
            _moving = true;
            State = Define.State.Moving;
        }
    }

    private void StartMovingToSitTable()
    {
        if (_sitTableWayPoint != null)
        {
            _currentDestination = _sitTableWayPoint.position;
            _moving = true;
            State = Define.State.Moving;
        }
    }


    private void HandleMovingToCashTable()
    {
        Vector3 destinationPos = _cashTableWayPoint.position + new Vector3(0, 0, _evenNPCList.Count * _lineSpacingZ);

        Debug.Log(destinationPos);
        Move(destinationPos);

        if (HasReachedDestination(destinationPos))
        {
            NPCCashLine(); // 줄 서기 처리
            State = Define.State.StackIdle;
            _moving = false;
        }
    }

    private void HandleMovingToCashTableToSit()
    {
        Vector3 destinationPos = _cashTableWayPointToSit.position + new Vector3(0, 0, _oddNPCList.Count * _lineSpacingZ);

        Move(destinationPos);

        if (HasReachedDestination(destinationPos))
        {
            NPCSitLine();
            State = Define.State.StackIdle;
            _moving = false;
        }
    
    }

    private void SortingToCashTable(int count)
    {
        Vector3 destinationPos = _cashTableWayPoint.position + new Vector3(0, 0, (count) * _lineSpacingZ);

        Debug.Log(destinationPos);
        Move(destinationPos);

        if (HasReachedDestination(destinationPos))
        {
            NPCCashLine(); // 줄 서기 처리
            State = Define.State.StackIdle;
            _moving = false;
        }
    }

    private void SortingToCashTableToSit()
    {
        Vector3 destinationPos = _cashTableWayPointToSit.position + new Vector3(0, 0, (_oddNPCList.Count - 1) * _lineSpacingZ);

        Move(destinationPos);

        if (HasReachedDestination(destinationPos))
        {
            NPCSitLine();
            State = Define.State.StackIdle;
            _moving = false;
        }

    }

    // 2 4 6 8 10 
    private void NPCCashLine()
    {
        // NPC가 줄 서기 위해 위치 조정
        Vector3 basePosition = _cashTableWayPoint.position;
        int index = _evenNPCList.IndexOf(this);

        if (index == -1)
        {
            _evenNPCList.Add(this);
            index = _evenNPCList.Count - 1;
        }

        Vector3 newPosition = basePosition + new Vector3(0, 0, index * _lineSpacingZ);
        transform.position = newPosition;
    }

    //1 3 5 7 9
    private void NPCSitLine()
    {
        // NPC가 줄 서기 위해 위치 조정
        Vector3 basePosition = _cashTableWayPointToSit.position;
        int index = _oddNPCList.IndexOf(this);

        if (index == -1)
        {
            _oddNPCList.Add(this);
            index = _oddNPCList.Count - 1;
        }

        Vector3 newPosition = basePosition + new Vector3(0, 0, index * _lineSpacingZ);
        transform.position = newPosition;
    }

    public void AddToCroassantStack(GameObject croasssant)
    {
        _croassantStack.Push(croasssant);
    }

    public GameObject RemoveToCroassantStack()
    {
        if (_croassantStack.Count > 0)
        {
            return _croassantStack.Pop();
        }
        return null;
    }

    public void AddToPaperBagStack(GameObject paperBag)
    {
        _paperStack.Push(paperBag);
    }

    public GameObject RemoveToPaperBagStack()
    {
        if (_paperStack.Count > 0)
        {
            return _paperStack.Pop();
        }
        return null;
    }

    private void RemoveList()
    {
        if (_oddNPCList.Contains(this))
        {
            _oddNPCList.Remove(this);
        }

        if (_evenNPCList.Contains(this))
        {
            _evenNPCList.Remove(this);
        }
    }

    public UI_Balloon GetUI()
    {
        return _ui;
    }

    public void AdjustNPCPositions()
    {
        if (State == Define.State.Moving)
            return;

        bool isOddList = false;
        // 퇴장한 NPC를 리스트에서 제거
        if (_oddNPCList.Contains(this))
        {
            isOddList = true;
            _oddNPCList.Remove(this);
            RepositionNPC(_oddNPCList, _cashTableWayPoint, isOddList);
        }
        else if (_evenNPCList.Contains(this))
        {
            if (_evenNPCList.Contains(this))
            {
                _evenNPCList.Remove(this);
                RepositionNPC(_evenNPCList, _cashTableWayPointToSit, isOddList);
            }
        }
    }

    private void RepositionNPC(List<NPCController> npcList, Transform baseWaypoint, bool isOddList)
    {
        for (int i = 0; i < npcList.Count; i++)
        {
            NPCController npc = npcList[i];

            npc.State = Define.State.StackMoving;
            _moving = true;
            SortingToCashTable(i);
        }
    }
}
