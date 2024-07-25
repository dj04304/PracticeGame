using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    // TODO 추후 옮길수도
    [SerializeField]
    protected float MoveSpeed;

    [SerializeField]
    protected Vector3 _destPos;

    [SerializeField]
    protected Define.State _state = Define.State.Idle;

    public virtual Define.State State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponentInChildren<Animator>();
            switch (_state)
            {
                case Define.State.Idle:
                    anim.SetBool("IsMove", false);
                    anim.SetBool("IsStackIdle", false);
                    anim.SetBool("IsStackMove", false);
                    //anim.CrossFade("WAIT", 0.1f);
                    break;
                case Define.State.Moving:
                    anim.SetBool("IsMove", true);
                    anim.SetBool("IsStackIdle", false);
                    anim.SetBool("IsStackMove", false);
                    //anim.CrossFade("RUN", 0.1f);
                    break;
                case Define.State.StackIdle:
                    anim.SetBool("IsMove", false);
                    anim.SetBool("IsStackIdle", true);
                    anim.SetBool("IsStackMove", false);
                    //anim.SetBool("attack", true);
                    //anim.CrossFade("ATTACK", 0.1f, -1, 0);
                    break;
                case Define.State.StackMoving:
                    anim.SetBool("IsMove", false);
                    anim.SetBool("IsStackMove", true);
                    anim.SetBool("IsStackIdle", false);
                    //anim.SetBool("attack", false);
                    break;
            }
        }

    }

    public Define.ObjectsType WorldObjectType { get; protected set; } = Define.ObjectsType.Unknown;

    private void Start()
    {
        Init();
    }

    void Update()
    {
        switch (State)
        {
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.StackIdle:
                UpdateStackIdle();
                break;
            case Define.State.StackMoving:
                UpdateStackMoving();
                break;
            case Define.State.Sitting:
                UpdateSitting();
                break;
        }

    }

    public abstract void Init();

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateStackIdle() { }
    protected virtual void UpdateStackMoving() { }
    protected virtual void UpdateSitting() { }

}
