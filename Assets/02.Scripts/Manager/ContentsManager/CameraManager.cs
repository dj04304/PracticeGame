using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.IO;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private List<CinemachineVirtualCamera> cameras; // CinemachineVirtualCamera ����Ʈ

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
    /// ī�޶� ü���� �Ű����� 1 -> �ٲ� ī�޶�, �Ű����� 2  -> ���� ī�޶� 
    /// </summary>
    /// <summary>
    /// ī�޶� ü���� �Ű����� 1 -> �ٲ� ī�޶�, �Ű����� 2  -> ���� ī�޶� 
    /// </summary>
    public static void OnChangedCineMachinePriority(string changeCamera, string curCamera)
    {
        if (cinViCameraDic.ContainsKey(changeCamera) && cinViCameraDic.ContainsKey(curCamera))
        {
            CinemachineVirtualCamera targetVirtualCamera = cinViCameraDic[changeCamera];
            CinemachineVirtualCamera curVirtualCamera = cinViCameraDic[curCamera];

            // �켱������ �����մϴ�.
            targetVirtualCamera.Priority = 11;
            curVirtualCamera.Priority = 10;
        }
    }

}
