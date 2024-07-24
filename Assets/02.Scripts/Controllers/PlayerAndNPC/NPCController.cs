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
    private Transform _breadWaypoint; // �� NPC���� �Ҵ�� �ϳ��� BreadWaypoint
    private Transform _cashTableWayPoint;
    private Transform _cashTableWayPointToSit;
    private Transform _sitTableWayPoint;
    private Transform _entranceWayPoint;

    [SerializeField]
    private NPCState _currentState;

    private Vector3 _currentDestination;

    private bool _moving = false;
    private bool _hasSeenTarget = false; // �̹� Ÿ���� �� �������� ����

    private int _mask = (1 << (int)Define.Layer.Basket | (1 << (int)Define.Layer.CashTable));
    private int _playerNpcMask = (1 << (int)Define.Layer.Player | (1 << (int)Define.Layer.NPC));

    private bool IsInMask(int layer) { return (_mask & (1 << layer)) != 0; }

    private Quaternion _lastKnownRotation; // ���������� �ٶ� ȸ���� ����

    public Transform HandPos => _handPos;

    private Stack<GameObject> _croassantStack = new Stack<GameObject>();
    private Stack<GameObject> _paperStack = new Stack<GameObject>();

    public static List<NPCController> _oddNPCList = new List<NPCController>();
    public static List<NPCController> _evenNPCList = new List<NPCController>();

    private readonly float _lineSpacingZ = 1.0f; // NPC �� ����

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

    // ������ ũ�ξƻ��� ���� ����
    public int RanCroassantMaxCount { get; set; }
    public int GetCroassantStackCount() { return _croassantStack.Count; }

    UI_Balloon _ui;

    public void SetWaypointsData(WaypointDatas waypointsData)
    {
        _firstWaypoint = waypointsData.FirstWaypoint;
        // �Ҵ�� �ϳ��� BreadWaypoint
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
        if (!_hasSeenTarget) // Ÿ���� �� ���� ������
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
                    Debug.Log($"Closest Collider: {closestCollider.gameObject.layer}"); // ����� �α�

                    // ���� ����� �繰�� �ٶ󺸰� ��
                    Vector3 direction = (closestCollider.transform.position - transform.position).normalized;

                    // ȸ�� ���
                    _lastKnownRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = _lastKnownRotation;

                    // UI ������Ʈ
                    if (closestCollider.gameObject.layer == (int)Define.Layer.Basket)
                        _ui.SetUIActive(true, bread: true, pay: false, chair : false);

                    _hasSeenTarget = true; // Ÿ���� �� ���·� ����
                }
            }
        }
        else
        {
            // Ÿ���� �� �Ŀ��� ��� ���� ���� ����
            transform.rotation = _lastKnownRotation;
        }
    }
    #endregion

    protected override void UpdateStackIdle()
    {
        if (!_hasSeenTarget) // Ÿ���� �� ���� ������
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
                    Debug.Log($"Closest Collider: {closestCollider.gameObject.layer}"); // ����� �α�

                    // ���� ����� �繰�� �ٶ󺸰� ��
                    Vector3 direction = (closestCollider.transform.position - transform.position).normalized;

                    // ȸ�� ���
                    _lastKnownRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = _lastKnownRotation;

                    // UI ������Ʈ
                    if (closestCollider.gameObject.layer == (int)Define.Layer.Basket)
                        _ui.SetUIActive(true, bread: true, pay: false, chair: false);

                    _hasSeenTarget = true; // Ÿ���� �� ���·� ����
                }
            }
        }
        else
        {
            // Ÿ���� �� �Ŀ��� ��� ���� ���� ����
            transform.rotation = _lastKnownRotation;
        }
    }

    // �Ϲ������� ������ ���� �� ����������, ���� ����
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

    // stack moving => ���� �տ� ��á����, ĳ�����̺� Ȥ�� �� ���̺�� �̵��� ���
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
                // Ÿ���� �ٶ󺸴� ���� �ʱ�ȭ
                _hasSeenTarget = false;

                Vector3 direction = (destination - transform.position).normalized;

                // ��ֹ� ȸ��
                Vector3 avoidDirection = Vector3.zero;
                float avoidDistance = 2.0f;  // ��ֹ� ȸ�� �Ÿ�
                Collider[] colliders = Physics.OverlapSphere(transform.position, avoidDistance, ~_playerNpcMask);

                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject != gameObject)  // �ڱ� �ڽ� ����
                    {
                        Vector3 awayFromObstacle = transform.position - collider.transform.position;
                        avoidDirection += awayFromObstacle.normalized / awayFromObstacle.magnitude;
                    }
                }

                if (avoidDirection != Vector3.zero)
                {
                    direction += avoidDirection * 0.5f;  // ȸ�� ���� ����
                    direction = direction.normalized;
                }

                if (direction != Vector3.zero)
                {
                    // �̵� ����� ȸ�� ������ ����Ͽ� NPC�� �̵���ŵ�ϴ�.
                    direction = new Vector3(direction.x, 0, direction.z).normalized; // Y���� 0���� ����
                    transform.position += direction * MoveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), MoveSpeed * Time.deltaTime);
                }
            }
        }
    }

    // ���� ����
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
        // ���� BreadWaypoint�� �̵� ����
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

    // ���� ���� ���� �ڵ�
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
            NPCCashLine(); // �� ���� ó��
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
            NPCCashLine(); // �� ���� ó��
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
        // NPC�� �� ���� ���� ��ġ ����
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
        // NPC�� �� ���� ���� ��ġ ����
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
        // ������ NPC�� ����Ʈ���� ����
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
