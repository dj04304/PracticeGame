using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.IO;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private List<CinemachineVirtualCamera> cameras; // CinemachineVirtualCamera 리스트

    public static Dictionary<string, CinemachineVirtualCamera> cinViCameraDic;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        cinViCameraDic = new Dictionary<string, CinemachineVirtualCamera>();

        foreach (var camera in cameras)
        {
            if (camera != null)
            {
                string keyName = camera.name;

                if (!cinViCameraDic.ContainsKey(keyName))
                {
                    cinViCameraDic.Add(keyName, camera);
                }
            }
        }
    }

    /// <summary>
    /// 카메라 체인지 매개변수 1 -> 바꿀 카메라, 매개변수 2  -> 현재 카메라 
    /// </summary>
    /// <summary>
    /// 카메라 체인지 매개변수 1 -> 바꿀 카메라, 매개변수 2  -> 현재 카메라 
    /// </summary>
    public static void OnChangedCineMachinePriority(string changeCamera, string curCamera)
    {
        if (cinViCameraDic.ContainsKey(changeCamera) && cinViCameraDic.ContainsKey(curCamera))
        {
            CinemachineVirtualCamera targetVirtualCamera = cinViCameraDic[changeCamera];
            CinemachineVirtualCamera curVirtualCamera = cinViCameraDic[curCamera];

            // 우선순위를 설정합니다.
            targetVirtualCamera.Priority = 11;
            curVirtualCamera.Priority = 10;
        }
    }

}
