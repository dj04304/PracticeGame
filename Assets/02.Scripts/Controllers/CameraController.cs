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

    private CinemachineVirtualCamera _virtualCamera = null;

    public void SetPlayer(GameObject player)
    {
        _player = player;

        // 플레이어가 설정되면 카메라의 Follow와 LookAt도 업데이트
        if (_virtualCamera != null && _player != null)
        {
            _virtualCamera.Follow = _player.transform;
            _virtualCamera.LookAt = _player.transform;
        }
    }

    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
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
