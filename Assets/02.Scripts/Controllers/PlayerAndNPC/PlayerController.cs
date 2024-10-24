using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerController : BaseController
{
    [SerializeField]
    private Vector3 _uiScale = new Vector3(0.02f, 0.02f, 0.02f);

    [SerializeField]
    private GameObject _handPos;

    [SerializeField]
    private GameObject _footArrow;

    [SerializeField]
    private GameObject _projectilePosition;

    [SerializeField]
    private int _croassantMaxCount = 8;

    [SerializeField]
    private VariableJoystick joy;

    [SerializeField]
    private GameObject joystickUI;


    public Transform HandPos => _handPos.transform;
    public Transform Projectile => _projectilePosition.transform;

    private Rigidbody _rigidbody;
    private UI_Max _ui_Max;


    private bool _isCompleteTutorial = false;
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

        joystickUI = GameManager.Instance.Resource.Instantiate("Joystick");

        GameObject joystickChild = Util.FindChild(joystickUI);
        joy = joystickChild.GetComponent<VariableJoystick>();

        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(joystickUI);


        if (evt != null)
        {
            evt.OnClickHandler += OnJoystickClick;
        }

        joystickUI.SetActive(false);

        //GameManager.Instance.Input.TouchAction -= OnTouchEvent;
        //GameManager.Instance.Input.TouchAction += OnTouchEvent;

        _rigidbody = GetComponent<Rigidbody>();

        _ui_Max = GameManager.Instance.UI.MakeWorldSpaceUI<UI_Max>(transform, null, null, _uiScale);
        
    }

    private void OnJoystickClick(PointerEventData data)
    {
        joystickUI.SetActive(true);
        joystickUI.transform.position = data.position;
    }


    private void FixedUpdate()
    {
        if (!_isCompleteTutorial)
        {
            Collider collider = GetComponent<Collider>();
            GameManager.Instance.Tutorial.HandleTriggerEnter(collider, _isCompleteTutorial, Define.NextTutorial.Oven);
            _isCompleteTutorial = true;
        }

        // 방향 벡터
        //Vector3 dir = _destPos - transform.position;
        Vector3 dir = new Vector3(joy.Horizontal, 0, joy.Vertical);
        dir.y = 0;

        // 도착시 IDLE
        if (dir.magnitude < 0.1f)
        {
            StopMoving();
        }
        else
        {
            if (_croassantStack.Count > 0)
            {
                State = Define.State.StackMoving;
            }
            else
            {
                State = Define.State.Moving;
            }

            // 방향 및 속도
            Vector3 moveDir = dir.normalized * MoveSpeed;
            _rigidbody.velocity = moveDir;

            // 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
        }
    }

    #region UPDATE MOVING
    //protected override void UpdateMoving()
    //{
    //    if (_moving)
    //    {

    //    }

    //    if (!_isCompleteTutorial)
    //    {
    //        Collider collider = GetComponent<Collider>();
    //        GameManager.Instance.Tutorial.HandleTriggerEnter(collider, _isCompleteTutorial, Define.NextTutorial.Oven);
    //        _isCompleteTutorial = true;
    //    }

    //    // 방향 벡터
    //    //Vector3 dir = _destPos - transform.position;
    //    Vector3 dir = new Vector3(joy.Horizontal, 0, joy.Vertical);
    //    dir.y = 0;

    //    // 도착시 IDLE
    //    if (dir.magnitude < 0.1f)
    //    {
    //        StopMoving();
    //    }
    //    else
    //    {
    //        if (_croassantStack.Count > 0)
    //        {
    //            State = Define.State.StackMoving;
    //        }
    //        else
    //        {
    //            State = Define.State.Moving;
    //        }

    //        // 방향 및 속도
    //        Vector3 moveDir = dir.normalized * MoveSpeed;
    //        _rigidbody.velocity = moveDir;

    //        // 회전
    //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
    //    }

    //}
    #endregion
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

    //#region TOUCH
    //void OnTouchEvent(Define.TouchEvent evt, Vector2 position)
    //{
    //    if (evt == Define.TouchEvent.Drag)
    //    {
    //        // 드래그 시 처리
    //        SetDestination(position);
    //        _moving = true;
    //    }
    //    else if (evt == Define.TouchEvent.DragEnd)
    //    {
    //        // 드래그 종료 시 처리
    //        StopMoving();
    //    }
    //}
    //#endregion

    //void OnTouchEvent(Define.TouchEvent evt)
    //{
    //    if (_croassantStack.Count > 0)
    //    {
    //        State = Define.State.StackMoving;
    //    }
    //    else
    //    {
    //        State = Define.State.Moving;
    //    }
    //}

    //#region SET DESTINATION
    //void SetDestination(Vector2 touchPosition)
    //{
    //    // 터치의 위치
    //    Ray ray = Camera.main.ScreenPointToRay(touchPosition);
    //    RaycastHit hitInfo;
    //    if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << (int)Define.Layer.Ground))
    //    {
    //        _destPos = hitInfo.point;
    //        if(_croassantStack.Count > 0)
    //        {
    //            State = Define.State.StackMoving;
    //        }
    //        else
    //        {
    //            State = Define.State.Moving;
    //        }
    //        _moving = true;
    //    }
    //}
    //#endregion
    //protected override void UpdateStackMoving()
    //{
    //    if (_croassantStack.Count > 0)
    //    {
    //        if (_moving)
    //        {
         
    //        }

    //        // 방향 벡터
    //        //Vector3 dir = _destPos - transform.position;
    //        Vector3 dir = new Vector3(joy.Horizontal, 0, joy.Vertical);
    //        dir.y = 0;

    //        // 도착시 IDLE
    //        if (dir.magnitude < 0.1f)
    //        {
    //            StopMoving();
    //        }
    //        else
    //        {
    //            // 방향 및 속도
    //            Vector3 moveDir = dir.normalized * MoveSpeed;
    //            _rigidbody.velocity = moveDir;

    //            // 회전
    //            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
    //        }
    //    }
    //}

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
    }

    public void MaxUISetActive(bool isMax)
    {
        _ui_Max.MaxCount(isMax);
    }
}
