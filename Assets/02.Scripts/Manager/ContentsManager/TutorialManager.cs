using System;
using System.Collections.Generic;
using UnityEngine;
public class TutorialManager
{
    public Transform GuideArrow; // 화살표를 위한 참조

    public Action OnReachOven;
    public Action OnReachBasket;
    public Action OnReachCounter;
    public Action OnReachCashPoint;
    public Action OnReachTrashPoint;
    public Action OnReachUnlockPoint;

    private TutorailPointDatas _tutorialDatas;
    private Define.NextTutorial _currentTargetType;
    private int _currentUnlockIndex;

    private int _mask = (1 << (int)Define.Layer.Player | 1 << (int)Define.Layer.NPC);


    // 목표의 완료 여부를 추적하기 위한 Dictionary
    private Dictionary<Define.NextTutorial, bool> _targetCompletionStatus = new Dictionary<Define.NextTutorial, bool>
    {
        { Define.NextTutorial.Oven, false },
        { Define.NextTutorial.Basket, false },
        { Define.NextTutorial.CashTable, false },
        { Define.NextTutorial.CashPoint, false },
        { Define.NextTutorial.Trash, false },
        { Define.NextTutorial.Unlock, false }
    };

    public void Init()
    {
        GameObject go = GameManager.Instance.Resource.Instantiate("Tutorial/Arrow");
        GuideArrow = go.transform;
    }

    public void SetTutorialPoint(TutorailPointDatas tutorialDatas)
    {
        _tutorialDatas = new TutorailPointDatas
        {
            OvenPoint = tutorialDatas.OvenPoint,
            BasketPoint = tutorialDatas.BasketPoint,
            CashTablePoint = tutorialDatas.CashTablePoint,
            CashPoint = tutorialDatas.CashPoint,
            TrashPoint = tutorialDatas.TrashPoint,
            UnLockPoint = new List<Transform>(tutorialDatas.UnLockPoint) // 리스트 복사
        };

        SetNextTarget(Define.NextTutorial.Oven); // 튜토리얼 시작 시 오븐을 첫 번째 목표로 설정

        _currentUnlockIndex = 0; // 초기화
    }

    public void HandleTriggerEnter(Collider other, bool isCompleteTutorial, Define.NextTutorial nextTarget)
    {
        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            Transform colliderTransform = other.transform;

            // 현재 목표와 일치하는지 확인
            switch (_currentTargetType)
            {
                case Define.NextTutorial.Oven:
                    if (!isCompleteTutorial)
                    {
                        OnReachOven?.Invoke();
                        _targetCompletionStatus[Define.NextTutorial.Oven] = true;
                        SetNextTarget(nextTarget);
                    }
                    break;
                case Define.NextTutorial.Basket:
                    if (!isCompleteTutorial)
                    {
                        OnReachBasket?.Invoke();
                        _targetCompletionStatus[Define.NextTutorial.Basket] = true;
                        SetNextTarget(nextTarget);
                    }
                    break;
                case Define.NextTutorial.CashTable:
                    if (!isCompleteTutorial)
                    {
                        OnReachCounter?.Invoke();
                        _targetCompletionStatus[Define.NextTutorial.CashTable] = true;
                        SetNextTarget(Define.NextTutorial.CashPoint);
                    }
                    break;
                case Define.NextTutorial.CashPoint:
                    if (!isCompleteTutorial)
                    {
                        OnReachCashPoint?.Invoke();
                        _targetCompletionStatus[Define.NextTutorial.CashPoint] = true;
                        SetNextTarget(nextTarget);
                    }
                    break;
                case Define.NextTutorial.Trash:
                    if (!isCompleteTutorial)
                    {
                        OnReachTrashPoint?.Invoke();
                        _targetCompletionStatus[Define.NextTutorial.Trash] = true;
                        SetNextTarget(nextTarget);
                    }
                    break;
                case Define.NextTutorial.Unlock:
                    if (_currentUnlockIndex < _tutorialDatas.UnLockPoint.Count && !isCompleteTutorial)
                    {
                        OnReachUnlockPoint?.Invoke();
                        _targetCompletionStatus[Define.NextTutorial.Unlock] = true;
                        _currentUnlockIndex++;
                        SetNextTarget(nextTarget);
                    }
                    break;
            }
        }
    }

    private void SetNextTarget(Define.NextTutorial nextType)
    {
        _currentTargetType = nextType;
        Transform targetTransform = null;
        Collider targetCollider = null;

        switch (nextType)
        {
            case Define.NextTutorial.Oven:
                targetTransform = _tutorialDatas.OvenPoint;
                targetCollider = _tutorialDatas.OvenPoint.GetComponent<Collider>();
                break;
            case Define.NextTutorial.Basket:
                targetTransform = _tutorialDatas.BasketPoint;
                targetCollider = _tutorialDatas.BasketPoint.GetComponent<Collider>();
                break;
            case Define.NextTutorial.CashTable:
                targetTransform = _tutorialDatas.CashTablePoint;
                targetCollider = _tutorialDatas.CashTablePoint.GetComponent<Collider>();
                break;
            case Define.NextTutorial.CashPoint:
                targetTransform = _tutorialDatas.CashPoint;
                targetCollider = _tutorialDatas.CashPoint.GetComponent<Collider>();
                break;
            case Define.NextTutorial.Trash:
                Sit sit = GameManager.Instance.Object.GetObj<Sit>();
                targetTransform = sit.Table.transform;
                targetCollider = sit.Table.GetComponent<Collider>();
                break;
            case Define.NextTutorial.Unlock:
                if (_tutorialDatas.UnLockPoint.Count > _currentUnlockIndex)
                {
                    targetTransform = _tutorialDatas.UnLockPoint[_currentUnlockIndex];
                    targetCollider = _tutorialDatas.UnLockPoint[_currentUnlockIndex].GetComponent<Collider>();
                }
                break;
        }

        if (targetTransform != null && targetCollider != null)
        {
            UpdateTutorialUI(targetCollider);
        }
    }

    private void UpdateTutorialUI(Collider targetCollider)
    {
        if (GuideArrow != null)
        {
            Vector3 colliderTop = targetCollider.bounds.center + Vector3.up * targetCollider.bounds.extents.y;
            GuideArrow.position = colliderTop;
        }
    }

    public void AddUnlockPoint(Transform unlockTransform)
    {
        if (unlockTransform != null)
        {
            _tutorialDatas.UnLockPoint.Add(unlockTransform);
        }

        foreach (Transform unlockPoint in _tutorialDatas.UnLockPoint)
        {
            if (unlockPoint != null)
            {
                Debug.Log($"Position: {unlockPoint.name}");
            }
        }

    }

}
