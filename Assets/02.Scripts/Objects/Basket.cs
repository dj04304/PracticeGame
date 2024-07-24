using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField]
    private Transform[] _breadAssignPoint;

    private Stack<GameObject> _croassantStack = new Stack<GameObject>();
    private Queue<NPCController> _npcQueue= new Queue<NPCController>();


    private int _playerMask = (1 << (int)Define.Layer.Player);
    private int _npcMask = (1 << (int)Define.Layer.NPC);

    private bool _isProcessingBread = false;
    private int _npcNum;

    private void OnTriggerEnter(Collider other)
    {
        #region Player Enter Trigger
        if ((1 << other.gameObject.layer & _playerMask) != 0)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if(playerController != null && playerController.GetCroassantStackCount() > 0) 
            {
                bool isMax = false;
                playerController.MaxUISetActive(isMax);

                // 플레이어가 빵 전달
                StartCoroutine(AssignBreadCo(playerController));
            }
        }
        #endregion

        if ((1 << other.gameObject.layer & _npcMask) != 0)
        {
            NPCController npcController = other.GetComponent<NPCController>();

            if (npcController != null)
            {
                if (!_npcQueue.Contains(npcController))
                {
                    _npcQueue.Enqueue(npcController);
                }
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if ((1 << other.gameObject.layer & _npcMask) != 0)
        {
            NPCController npcController = other.GetComponent<NPCController>();

            if (npcController != null && !_isProcessingBread)
            {
                if (_croassantStack.Count > 0)
                {
                    _isProcessingBread = true;
                    StartCoroutine(ProcessBreadToNPCCo(npcController));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((1 << other.gameObject.layer & _npcMask) != 0)
        {
            NPCController npcController = other.GetComponent<NPCController>();

            if (npcController != null)
            {

            }
        }
    }


    private IEnumerator ProcessBreadToNPCCo(NPCController npcController)
    {

        yield return new WaitForSeconds(1);


        while (_croassantStack.Count > 0 && _npcQueue.Count > 0)
        {
            NPCController npc = _npcQueue.Peek();

            //npc 의 각 크로아상 요구수
            int requestCount = npc.RanCroassantMaxCount - npc.GetCroassantStackCount();

            while (requestCount > 0 && _croassantStack.Count > 0)
            {
                npc.State = Define.State.StackIdle;

                GameObject croassant = _croassantStack.Pop();
                npc.AddToCroassantStack(croassant);

                CroassantProjectile projectile = croassant.GetComponent<CroassantProjectile>();

                int numberOfCroassant = npc.GetCroassantStackCount();
                float baseHeight = 0.3f;

                Vector3 startPosition = croassant.transform.position;
                Vector3 targetPosition = npc.HandPos.position;

                if (numberOfCroassant > 1)
                {
                    startPosition.y += baseHeight * numberOfCroassant;
                    targetPosition.y += baseHeight * numberOfCroassant;
                }

                projectile.Initialize(startPosition, targetPosition, npc.HandPos, Define.ArriveType.StackType, numberOfCroassant);

                yield return new WaitUntil(() => projectile.HasArrived());

                requestCount--;
                // UI 갱신
                UI_Balloon uiBalloon = npc.GetUI();
                uiBalloon.UpdateText(requestCount);
            }

            if(requestCount <= 0)
            {
                // 처음 들고온 수가 1이면 sit으로 2면 cash로
                if(npc.RanCroassantMaxCount == 1)
                {
                    UI_Balloon uiBalloon = npc.GetUI();
                    uiBalloon.SetUIActive(true, bread: false, pay: false, chair: true);
                    npc.SelectAtFirstWayPoint();
                }
                else
                {
                    UI_Balloon uiBalloon = npc.GetUI();
                    uiBalloon.SetUIActive(true, bread: false, pay: true, chair: false);

                    // npc의 상태를 바꿔줌
                    npc.SelectAtFirstWayPoint();
                }

                // 현재 NPC의 모든 빵을 처리한 경우
                _npcQueue.Dequeue();

            }

            // NPC가 없거나 빵이 없으면 처리 중지
            if (_npcQueue.Count == 0 || _croassantStack.Count == 0)
                break;

        }


        _isProcessingBread = false;
    }

    private IEnumerator AssignBreadCo(PlayerController playerController)
    {
        if (playerController.GetCroassantStackCount() <= 0)
            yield break;

       
        // 빵
        Stack<GameObject> croassantStack = playerController.CroassantStack();
       
        // transform을 List에 담음
        List<Transform> availablePoints = new List<Transform>();
        foreach (var point in _breadAssignPoint)
        {
            if (point.childCount == 0) // 빈 지점만 추가
                availablePoints.Add(point);
            
        }

        // 비어있는 trasnform의 수
        int availableCount = availablePoints.Count;

        // 크로아상 스택 개수, 비어있는 transform의 수 중 작은값으로 계산해서 반복문 최소화 및 빈칸에만 전달
        int assignCount = Mathf.Min(croassantStack.Count, availableCount);

        for (int i = 0; i < assignCount; i++)
        {
            GameObject croassant = croassantStack.Pop();
            CroassantProjectile projectile = croassant.GetComponent<CroassantProjectile>();

            // NPC에게 전달하기 위함
            _croassantStack.Push(croassant);

            int numberOfCroassant = playerController.GetCroassantStackCount();

            Transform targetPoint = availablePoints[i];
            Vector3 startPosition = croassant.transform.position;
            Vector3 targetPosition = targetPoint.position;

            projectile.Initialize(startPosition, targetPosition, targetPoint, Define.ArriveType.NomalType, numberOfCroassant);

            // 발사가 완료될 때까지 대기
            yield return new WaitUntil(() => projectile.HasArrived());

            croassant.transform.SetParent(targetPoint);
        }
        

    }
}
