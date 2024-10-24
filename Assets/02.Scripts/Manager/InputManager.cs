using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action<Define.TouchEvent> TouchAction = null;

    bool _pressed = false; // �Է� ��ġ����
    bool _dragged = false; // �巡�� ����
    Vector2 _startPosition; // ��ġ�� ��ġ (�巡�� ���� �Ǵ�)
    float _dragThreshold = 10f; // �巡�� �ּ� �Ÿ�

    //public void OnUpdate()
    //{
    //    // ��ġ �Է�
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);
    //        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
    //            return;

    //        if (TouchAction != null)
    //        {
    //            switch (touch.phase)
    //            {
    //                case TouchPhase.Began:
    //                    _pressed = true;
    //                    _startPosition = touch.position;
    //                    _dragged = false;
    //                    break;
    //                case TouchPhase.Moved:
    //                    if (_pressed && !_dragged)
    //                    {
    //                        if (Vector2.Distance(_startPosition, touch.position) > _dragThreshold)
    //                        {
    //                            _dragged = true;
    //                            TouchAction?.Invoke(Define.TouchEvent.Drag, touch.position);
    //                        }
    //                    }
    //                    else if (_pressed && _dragged)
    //                    {
    //                        TouchAction?.Invoke(Define.TouchEvent.Drag, touch.position);
    //                    }
    //                    break;
    //                case TouchPhase.Ended:
    //                    if (_pressed && !_dragged)
    //                    {
    //                        TouchAction?.Invoke(Define.TouchEvent.Click, touch.position);
    //                    }
    //                    else if (_pressed && _dragged)
    //                    {
    //                        TouchAction?.Invoke(Define.TouchEvent.DragEnd, touch.position);
    //                    }
    //                    _pressed = false;
    //                    _dragged = false;
    //                    break;
    //            }
    //        }
    //    }
    //    // ���콺 �Է�
    //    else
    //    {
    //        if (EventSystem.current.IsPointerOverGameObject())
    //            return;

    //        if (TouchAction != null)
    //        {
    //            if (Input.GetMouseButton(0))
    //            {
    //                // ó�� �������� ��
    //                if (!_pressed)
    //                {
    //                    _pressed = true;
    //                    _startPosition = Input.mousePosition;
    //                    _dragged = false;
    //                }
    //                else if (_pressed && !_dragged)
    //                {
    //                    // ��ġ�� ������, ���� ���콺�� ��ġ�� _dragThreshold �� Ŭ ��� ��, ���� �Ÿ��� �������� ��
    //                    if (Vector2.Distance(_startPosition, Input.mousePosition) > _dragThreshold)
    //                    {
    //                        _dragged = true;
    //                        TouchAction?.Invoke(Define.TouchEvent.Drag, Input.mousePosition);
    //                    }
    //                }
    //                else if (_pressed && _dragged)
    //                {
    //                    TouchAction?.Invoke(Define.TouchEvent.Drag, Input.mousePosition);
    //                }
    //            }
    //            else
    //            {
    //                if (_pressed)
    //                {
    //                    if (!_dragged)
    //                    {
    //                        TouchAction?.Invoke(Define.TouchEvent.Click, Input.mousePosition);
    //                    }
    //                    else
    //                    {
    //                        TouchAction?.Invoke(Define.TouchEvent.DragEnd, Input.mousePosition);
    //                    }
    //                    _pressed = false;
    //                    _dragged = false;
    //                }
    //            }
    //        }
    //    }
    //}

    public void OnUpdate()
    {
        // ��ġ �Է� ����
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            if (touch.phase == TouchPhase.Began)
            {
                _pressed = true;
                TouchAction?.Invoke(Define.TouchEvent.Touch);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _pressed = false;
                TouchAction?.Invoke(Define.TouchEvent.TouchEnd);
            }
        }
        // ���콺 �Է� ����
        else
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                _pressed = true;
                TouchAction?.Invoke(Define.TouchEvent.Touch);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _pressed = false;
                TouchAction?.Invoke(Define.TouchEvent.TouchEnd);
            }
        }
    }

    public void Clear()
    {
        TouchAction = null;
    }
}
