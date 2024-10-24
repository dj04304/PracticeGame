using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action<Define.TouchEvent> TouchAction = null;

    bool _pressed = false; // 입력 터치여부
    bool _dragged = false; // 드래그 여부
    Vector2 _startPosition; // 터치의 위치 (드래그 여부 판단)
    float _dragThreshold = 10f; // 드래그 최소 거리

    //public void OnUpdate()
    //{
    //    // 터치 입력
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
    //    // 마우스 입력
    //    else
    //    {
    //        if (EventSystem.current.IsPointerOverGameObject())
    //            return;

    //        if (TouchAction != null)
    //        {
    //            if (Input.GetMouseButton(0))
    //            {
    //                // 처음 눌러졌을 때
    //                if (!_pressed)
    //                {
    //                    _pressed = true;
    //                    _startPosition = Input.mousePosition;
    //                    _dragged = false;
    //                }
    //                else if (_pressed && !_dragged)
    //                {
    //                    // 터치된 시점과, 현재 마우스의 위치가 _dragThreshold 다 클 경우 즉, 둘의 거리가 벌어졌을 때
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
        // 터치 입력 감지
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
        // 마우스 입력 감지
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
