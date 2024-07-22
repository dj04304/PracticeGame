using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField]
    private Transform[] _breadAssignPoint;

    private Stack<GameObject> _croassantStack = new Stack<GameObject>();
    private int _maxCount;

    private int _mask = (1 << (int)Define.Layer.Player);

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if(playerController != null && playerController.GetCroassantStackCount() > 0) 
            {
                bool isMax = false;
                playerController.MaxUISetActive(isMax);
                _maxCount = _breadAssignPoint.Length;

                Debug.Log("실행");
                // 플레이어가 빵 전달
                StartCoroutine(AssignBreadCo(playerController));

                // NPC에게 빵 나눠주기
            }
        }
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

            Transform targetPoint = availablePoints[i];
            Vector3 startPosition = croassant.transform.position;
            Vector3 targetPosition = targetPoint.position;

            projectile.Initialize(startPosition, targetPosition, targetPoint, Define.ArriveType.Basket);

            // 발사가 완료될 때까지 대기
            yield return new WaitUntil(() => projectile.HasArrived());

            croassant.transform.SetParent(targetPoint);
        }
        

    }
}
