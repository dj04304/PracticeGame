using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    [SerializeField]
    private GameObject _breadPosition;

    [SerializeField]
    private GameObject _projectilePosition;

    public Transform BreadPosition => _breadPosition.transform;
    public Transform Projectile => _projectilePosition.transform;

    private Rigidbody _rigidbody;
    private UI_Max _ui_Max;

    private bool _moving = false;
    private bool _isInTrigger = false;

    private Stack<GameObject> _croassantStack = new Stack<GameObject>(); // �� ����

    public int GetCroassantStackCount() { return _croassantStack.Count; }

    public override void Init()
    {
        WorldObjectType = Define.ObjectsType.Player;
        State = Define.State.Idle;
        MoveSpeed = 5.0f;

        GameManager.Instance.Input.TouchAction -= OnTouchEvent;
        GameManager.Instance.Input.TouchAction += OnTouchEvent;

        _rigidbody = GetComponent<Rigidbody>();

        _ui_Max = GameManager.Instance.UI.MakeWorldSpaceUI<UI_Max>(transform);
    }

    protected override void UpdateMoving()
    {
        if (_moving)
        {
            // ���� ����
            Vector3 dir = _destPos - transform.position;
            dir.y = 0;

            // ������ IDLE
            if (dir.magnitude < 0.1f)
            {
                StopMoving();
            }
            else
            {
                // ���� �� �ӵ�
                Vector3 moveDir = dir.normalized * MoveSpeed;
                _rigidbody.velocity = moveDir;

                // ȸ��
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
            }
        }
    }

    private void StopMoving()
    {
        _moving = false;
        _rigidbody.velocity = Vector3.zero; // ����

        if (_isInTrigger)
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
            // �巡�� �� ó��
            SetDestination(position);
            _moving = true;
        }
        else if (evt == Define.TouchEvent.DragEnd)
        {
            // �巡�� ���� �� ó��
            StopMoving();
        }
    }

    void SetDestination(Vector2 touchPosition)
    {
        // ��ġ�� ��ġ
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << (int)Define.Layer.Ground))
        {
            _destPos = hitInfo.point;
            State = Define.State.Moving;
            _moving = true;
        }
    }

    protected override void UpdateStackIdle()
    {
        Debug.Log("����");
        // �߰����� ���� ���� ����
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
        UpdateCroassantPositions();
    }

    private void UpdateCroassantPositions()
    {
        float heightIncrement = 0.3f;
        int count = _croassantStack.Count;
        float startY = BreadPosition.position.y;

        for (int i = 0; i < count; i++)
        {
            GameObject croassant = _croassantStack.ToArray()[i]; 
            Vector3 positionOffset = new Vector3(0, heightIncrement * i, 0);
            croassant.transform.position = BreadPosition.position + positionOffset;
        }
    }
}