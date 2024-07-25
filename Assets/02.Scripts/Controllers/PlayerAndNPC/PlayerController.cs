using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    [SerializeField]
    private Vector3 _uiScale = new Vector3(0.02f, 0.02f, 0.02f);

    [SerializeField]
    private GameObject _handPos;

    [SerializeField]
    private GameObject _projectilePosition;

    [SerializeField]
    private int _croassantMaxCount = 8;

    public Transform HandPos => _handPos.transform;
    public Transform Projectile => _projectilePosition.transform;

    private Rigidbody _rigidbody;
    private UI_Max _ui_Max;


    private bool _moving = false;
    private bool _isInTrigger = false;


    private Stack<GameObject> _croassantStack = new Stack<GameObject>(); // 빵 스택

    public Stack<GameObject> CroassantStack() {  return _croassantStack; }

    public int GetCroassantStackCount() { return _croassantStack.Count; }
    public int CroassantMaxCount() { return _croassantMaxCount; }

    public override void Init()
    {
        WorldObjectType = Define.ObjectsType.Player;
        State = Define.State.Idle;
        MoveSpeed = 5.0f;

        GameManager.Instance.Input.TouchAction -= OnTouchEvent;
        GameManager.Instance.Input.TouchAction += OnTouchEvent;

        _rigidbody = GetComponent<Rigidbody>();

        _ui_Max = GameManager.Instance.UI.MakeWorldSpaceUI<UI_Max>(transform, null, null, _uiScale);
    }

    protected override void UpdateMoving()
    {
        if (_moving)
        {
            // 방향 벡터
            Vector3 dir = _destPos - transform.position;
            dir.y = 0;

            // 도착시 IDLE
            if (dir.magnitude < 0.1f)
            {
                StopMoving();
            }
            else
            {
                // 방향 및 속도
                Vector3 moveDir = dir.normalized * MoveSpeed;
                _rigidbody.velocity = moveDir;

                // 회전
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            }
        }
    }

    private void StopMoving()
    {
        _moving = false;
        _rigidbody.velocity = Vector3.zero; // 정지

        if (_croassantStack.Count > 0)
        {
            State = Define.State.StackIdle;
        }
        else
        {
            State = Define.State.Idle;
        }
    }

    void OnTouchEvent(Define.TouchEvent evt, Vector2 position)
    {
        if (evt == Define.TouchEvent.Drag)
        {
            // 드래그 시 처리
            SetDestination(position);
            _moving = true;
        }
        else if (evt == Define.TouchEvent.DragEnd)
        {
            // 드래그 종료 시 처리
            StopMoving();
        }
    }

    void SetDestination(Vector2 touchPosition)
    {
        // 터치의 위치
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << (int)Define.Layer.Ground))
        {
            _destPos = hitInfo.point;
            if(_croassantStack.Count > 0)
            {
                State = Define.State.StackMoving;
            }
            else
            {
                State = Define.State.Moving;
            }
            _moving = true;
        }
    }

    protected override void UpdateStackIdle()
    {

        
    }

    protected override void UpdateStackMoving()
    {
        if (_croassantStack.Count > 0)
        {
            if (_moving)
            {
                // 방향 벡터
                Vector3 dir = _destPos - transform.position;
                dir.y = 0;

                // 도착시 IDLE
                if (dir.magnitude < 0.1f)
                {
                    StopMoving();
                }
                else
                {
                    // 방향 및 속도
                    Vector3 moveDir = dir.normalized * MoveSpeed;
                    _rigidbody.velocity = moveDir;

                    // 회전
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
                }
            }
        }
    }

    public void SetInIdleTrigger(bool inTrigger)
    {
        _isInTrigger = inTrigger;
        if (!_moving)
        {
            State = inTrigger ? Define.State.StackIdle : Define.State.Idle;
        }
    }

    public void AddToCroassantStack(GameObject croassant)
    {
        _croassantStack.Push(croassant);
        // UpdateCroassantPositions();
    }

    //private void UpdateCroassantPositions()
    //{
        //float heightIncrement = 0.3f;
        //int count = _croassantStack.Count;
        //float startY = HandPos.position.y;

        //for (int i = 0; i < count; i++)
        //{
        //    GameObject croassant = _croassantStack.ToArray()[i]; 
        //    Vector3 positionOffset = new Vector3(0, heightIncrement * i, 0);
        //    croassant.transform.position = HandPos.position + positionOffset;
        //}
    //}

    public void MaxUISetActive(bool isMax)
    {
        _ui_Max.MaxCount(isMax);
    }
}
