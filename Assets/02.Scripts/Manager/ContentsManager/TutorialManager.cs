using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager
{
    public Transform GuideArrow; // 튜토리얼 화살표
    public GameObject FootArrow;  // 플레이어 발 아래에 위치할 화살표

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

    private GameObject _player;

    public GameObject GetPlayer {  get { return _player; } }
    public GameObject SetPlayer(GameObject player)
    {
        _player = player;

        if (_player != null)
        {
            // "Arrow" 태그를 가진 자식 객체를 찾습니다
            GameObject[] arrows = GameObject.FindGameObjectsWithTag("Arrow");
            foreach (GameObject arrow in arrows)
            {
                if (arrow.transform.IsChildOf(_player.transform))
                {
                    FootArrow = arrow;
                    break;
                }
            }

            if (FootArrow != null)
            {
                FootArrow.SetActive(true);
                PositionFootArrow();
            }
        }

        return _player;
    }

    private Dictionary<Define.NextTutorial, bool> _targetCompletionStatusDic = new Dictionary<Define.NextTutorial, bool>
    {
        { Define.NextTutorial.Oven, false },
        { Define.NextTutorial.Basket, false },
        { Define.NextTutorial.CashTable, false },
        { Define.NextTutorial.CashPoint, false },
        { Define.NextTutorial.Trash, false },
    };

    // Unlock 포인트들의 완료 여부를 추적하기 위한 Dictionary
    private Dictionary<Transform, bool> _unlockCompletionStatusDic = new Dictionary<Transform, bool>();

    public void Init()
    {
        GameObject root = GameObject.Find("@Tutorial");
        if (root == null)
        {
            root = new GameObject { name = "@Tutorial" };
            UnityEngine.Object.DontDestroyOnLoad(root);
            root.GetOrAddComponent<TutorialManagerEx>();

            GameObject go = GameManager.Instance.Resource.Instantiate("Tutorial/Arrow");
            GuideArrow = go.transform;
            if (FootArrow != null)
            {
                FootArrow.gameObject.SetActive(true);
                PositionFootArrow();
            }
        }
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
            UnLockPoint = new List<Transform>(tutorialDatas.UnLockPoint)
        };

        foreach (var unlockPoint in _tutorialDatas.UnLockPoint)
        {
            _unlockCompletionStatusDic[unlockPoint] = false;
        }

        SetNextTarget(Define.NextTutorial.Oven);

        _currentUnlockIndex = 0;
    }
    #region TUTORIAL TRIGGER
    public void HandleTriggerEnter(Collider other, bool isCompleteTutorial, Define.NextTutorial nextTarget)
    {
        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            Transform colliderTransform = other.transform;

            switch (_currentTargetType)
            {
                case Define.NextTutorial.Oven:
                    if (!isCompleteTutorial)
                    {
                        OnReachOven?.Invoke();
                        _targetCompletionStatusDic[Define.NextTutorial.Oven] = true;
                        SetNextTarget(nextTarget);
                    }
                    break;
                case Define.NextTutorial.Basket:
                    if (!isCompleteTutorial)
                    {
                        OnReachBasket?.Invoke();
                        _targetCompletionStatusDic[Define.NextTutorial.Basket] = true;
                        SetNextTarget(nextTarget);
                    }
                    break;
                case Define.NextTutorial.CashTable:
                    if (!isCompleteTutorial)
                    {
                        OnReachCounter?.Invoke();
                        _targetCompletionStatusDic[Define.NextTutorial.CashTable] = true;
                        SetNextTarget(Define.NextTutorial.CashPoint);
                    }
                    break;
                case Define.NextTutorial.CashPoint:
                    if (!isCompleteTutorial)
                    {
                        OnReachCashPoint?.Invoke();
                        _targetCompletionStatusDic[Define.NextTutorial.CashPoint] = true;
                        SetNextTarget(nextTarget);
                    }
                    break;
                case Define.NextTutorial.Trash:
                    if (!isCompleteTutorial)
                    {
                        OnReachTrashPoint?.Invoke();
                        _targetCompletionStatusDic[Define.NextTutorial.Trash] = true;
                        SetNextTarget(nextTarget);
                    }
                    break;
                case Define.NextTutorial.Unlock:
                    if (_currentUnlockIndex < _tutorialDatas.UnLockPoint.Count && !isCompleteTutorial)
                    {
                        Debug.Log(_currentUnlockIndex);
                        OnReachUnlockPoint?.Invoke();
                        _unlockCompletionStatusDic[_tutorialDatas.UnLockPoint[_currentUnlockIndex]] = true;
                        _currentUnlockIndex++;
                        SetNextTarget(nextTarget);
                    }
                    break;
            }
        }
    }
    #endregion

    #region SET NEXT TUTORIAL
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
    #endregion
    private void UpdateTutorialUI(Collider targetCollider)
    {
        if (GuideArrow != null)
        {
            Vector3 colliderTop = targetCollider.bounds.center + Vector3.up * targetCollider.bounds.extents.y;
            GuideArrow.position = colliderTop;
        }
    }

    private void PositionFootArrow()
    {
        if (_player != null && FootArrow != null)
        {
            Vector3 playerPosition = _player.transform.position;
            FootArrow.transform.position = playerPosition + new Vector3(0, 0, 1.5f);
            FootArrow.transform.rotation = Quaternion.Euler(90, 90, 270);
        }
    }

    public void AddUnlockPoint(Transform unlockTransform)
    {
        if (unlockTransform != null)
        {
            _tutorialDatas.UnLockPoint.Add(unlockTransform);
        }
    }

    public void AddUplockDicPoint(Transform unlockTransform)
    {
       _unlockCompletionStatusDic.Add(unlockTransform, false);
    }

    public void SetGuideArrowActive(bool isActive)
    {
        if (GuideArrow != null)
        {
            GuideArrow.gameObject.SetActive(isActive);
        }

        if (FootArrow != null)
        {
            FootArrow.gameObject.SetActive(isActive);
        }
    }
}
