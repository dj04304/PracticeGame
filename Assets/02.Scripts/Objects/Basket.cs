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

                Debug.Log("����");
                // �÷��̾ �� ����
                StartCoroutine(AssignBreadCo(playerController));

                // NPC���� �� �����ֱ�
            }
        }
    }

    private IEnumerator AssignBreadCo(PlayerController playerController)
    {
        if (playerController.GetCroassantStackCount() <= 0)
            yield break;

       
        // ��
        Stack<GameObject> croassantStack = playerController.CroassantStack();

        // transform�� List�� ����
        List<Transform> availablePoints = new List<Transform>();
        foreach (var point in _breadAssignPoint)
        {
            if (point.childCount == 0) // �� ������ �߰�
                availablePoints.Add(point);
            
        }

        // ����ִ� trasnform�� ��
        int availableCount = availablePoints.Count;

        // ũ�ξƻ� ���� ����, ����ִ� transform�� �� �� ���������� ����ؼ� �ݺ��� �ּ�ȭ �� ��ĭ���� ����
        int assignCount = Mathf.Min(croassantStack.Count, availableCount);

        for (int i = 0; i < assignCount; i++)
        {
            GameObject croassant = croassantStack.Pop();
            CroassantProjectile projectile = croassant.GetComponent<CroassantProjectile>();

            Transform targetPoint = availablePoints[i];
            Vector3 startPosition = croassant.transform.position;
            Vector3 targetPosition = targetPoint.position;

            projectile.Initialize(startPosition, targetPosition, targetPoint, Define.ArriveType.Basket);

            // �߻簡 �Ϸ�� ������ ���
            yield return new WaitUntil(() => projectile.HasArrived());

            croassant.transform.SetParent(targetPoint);
        }
        

    }
}
