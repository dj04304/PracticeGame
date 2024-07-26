//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CameraController : MonoBehaviour
//{
//    [SerializeField]
//    Define.CameraMode _mode = Define.CameraMode.QuaterView;

//    [SerializeField]
//    Vector3 _delta = new Vector3(-10.0f, 8.0f, 0f);

//    [SerializeField]
//    GameObject _player = null;

//    public void SetPalyer(GameObject player) { _player = player; }

//    void Start()
//    {

//    }

//    void LateUpdate()
//    {
//        if(_mode == Define.CameraMode.QuaterView)
//        {
//            if (_player.IsValid() == false)
//            {
//                return;
//            }

//            transform.position = _player.transform.position + _delta;
//            transform.LookAt(_player.transform);
//        }
//    }

//    public void SetQuaterView(Vector3 delta)
//    {
//        _mode = Define.CameraMode.QuaterView;
//        _delta = delta;
//    }
//}
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.QuaterView;

    [SerializeField]
    Vector3 _delta = new Vector3(-10.0f, 8.0f, 0f);

    [SerializeField]
    GameObject _player = null;

    [SerializeField]
    CinemachineVirtualCamera _virtualCamera = null; // Cinemachine Virtual Camera

    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    void Start()
    {
        // 초기 설정이 필요한 경우 추가
    }

    void LateUpdate()
    {
        if (_mode == Define.CameraMode.QuaterView)
        {
            if (_player == null || _virtualCamera == null)
            {
                return;
            }

            // 가상 카메라의 위치와 회전을 설정
            _virtualCamera.transform.position = _player.transform.position + _delta;
            _virtualCamera.transform.LookAt(_player.transform);
        }
    }

    public void SetQuaterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuaterView;
        _delta = delta;
    }
}
