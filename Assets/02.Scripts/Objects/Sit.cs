using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Sit : Obj_Base
{
    enum GameObjects
    {
        Chair,
        Table,
        MoneyStorage,
    }

    public GameObject Chair;
    public GameObject Table;
    public GameObject MoneyStorage;

    private bool _isTrashCleared = false;

    private int _npcMask = (1 << (int)Define.Layer.NPC);
    private int _playerMask = (1 << (int)Define.Layer.Player);

    GameObject _croassant;
    Collider _tableCollider;
    public GameObject GetCroassnat() { return _croassant; }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Chair = GetObject((int)GameObjects.Chair);
        Table = GetObject((int)GameObjects.Table);
        MoneyStorage = GetObject((int)GameObjects.MoneyStorage);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _playerMask) != 0 && !_isTrashCleared)
        {
            DestroyTrashOnTable();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        HandleTrigger(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if ((1 << other.gameObject.layer & _playerMask) != 0)
        {
            // �÷��� ����
            _isTrashCleared = false;
        }
    }

    private void DestroyTrashOnTable()
    {
        if (Table != null)
        {
            _tableCollider = Table.GetComponent<Collider>();
            
            // Table�� ��� �ڽ� ������Ʈ�� ������ ����Ʈ�� ����ϴ�.
            List<Transform> trashChildren = new List<Transform>();

            // "Trash" �±׸� ���� �ڽ� ������Ʈ�� ����Ʈ�� �߰��մϴ�.
            foreach (Transform child in Table.transform)
            {
                if (child.CompareTag("Trash"))
                {
                    trashChildren.Add(child);
                }
            }

            // ����� �ڽ� ������Ʈ���� ��ȸ�ϸ� �ı��մϴ�.
            foreach (Transform trash in trashChildren)
            {
                GameManager.Instance.Resource.Destroy(trash.gameObject);

                Vector3 tablePosition = new Vector3(0, 2.0f, 0);
                Debug.Log(tablePosition);
                GameManager.Instance.Particle.Play("VFX_Clean", Table.transform, tablePosition);
            }

            // �÷��� ����
            _isTrashCleared = true;
        }
    }

    private void HandleTrigger(Collider other)
    {
        // ���̺��� �����ϴ� ���̾ üũ
        if ((1 << other.gameObject.layer & _npcMask) != 0)
        {
            NPCController npc = other.GetComponent<NPCController>();
            if (npc != null && npc.State == Define.State.Sitting)
            {
                Transform closestTable = null;
                float closestDistance = float.MaxValue;

                // ���� �ݶ��̴��� bounds�� ����Ͽ� Ʈ���� ���� ���� ��� ���̺� ã��
                Collider[] collidersInTrigger = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents, transform.rotation);

                foreach (Collider col in collidersInTrigger)
                {
                    if (col.CompareTag("Table") || col.gameObject.name.Contains("Table"))
                    {
                        float distanceToTable = Vector3.Distance(npc.transform.position, col.transform.position);

                        if (distanceToTable < closestDistance)
                        {
                            closestDistance = distanceToTable;
                            closestTable = col.transform;
                        }
                    }
                }

                if (closestTable != null)
                {

                    if(npc.GetCroassantStackCount() > 0)
                    {
                        StartCoroutine(SitTableToEat(npc));
                    }

                    // NPC�� ���� ����� ���̺��� �ٶ󺸵��� ����
                    Vector3 directionToTable = (closestTable.position - npc.transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTable);
                    npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, targetRotation, Time.deltaTime * 5.0f);
                }
            }
        }
    }

    private IEnumerator SitTableToEat(NPCController npc)
    {
        if (npc.GetCroassantStackCount() <= 0)
        {
            yield break;
        }

        // �� ���� ��������
        Stack<GameObject> croassantStack = npc.GetCroassantStack();

        GameObject croassant = croassantStack.Peek();

        StartCoroutine(LaunchCroassant(npc, croassant));

    }




    private IEnumerator LaunchCroassant(NPCController npc, GameObject croassant)
    {
        if (croassant == null) yield break;

        _tableCollider = Table.GetComponent<Collider>();
        Vector3 _startPosition = npc.HandPos.position;
        Vector3 _targetPosition = _tableCollider.bounds.center + Vector3.up * (_tableCollider.bounds.extents.y);

        float timeElapsed = 0f;
        float duration = 0.2f;
        float height = 0.3f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            float parabolaHeight = Mathf.Sin(t * Mathf.PI) * height;

            // �� ��ġ ���
            Vector3 newPosition = Vector3.Lerp(_startPosition, _targetPosition, t);
            newPosition.y += parabolaHeight;

            // ��ü ��ġ ����
            croassant.transform.position = newPosition;

            yield return null;
        }

        // ���� ��ġ ����
        croassant.transform.position = _targetPosition;

        HandleArrival(croassant, npc);
    }

    private void HandleArrival(GameObject croassant, NPCController npc)
    {
        if (npc != null)
        {
            if(npc.GetCroassantStackCount() > 0)
                npc.GetCroassantStack().Pop();

            croassant.transform.SetParent(_tableCollider.transform);
            _croassant = croassant;
        }
    }
}
