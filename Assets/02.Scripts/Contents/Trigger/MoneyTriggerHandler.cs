using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTriggerHandler : MonoBehaviour
{
    [SerializeField]
    private BaseStackMoney _moneyStorage;

    private GameObject _player;
    private CashTable _cashTable;
    //private MoneySpawningPool _moneyPool;

    public List<GameObject> MoneyList = new List<GameObject>();

    private int _playerMask = (1 << (int)Define.Layer.Player);

    private bool _isCompleteTutorial = false;

    public void Init(CashTable cashTable)
    {
        _cashTable = cashTable;
   //     _moneyPool = cashTable.GetComponent<MoneySpawningPool>();
        _player = GameManager.Instance.Spawn.GetPlayer();

        Collider collider = GetComponent<Collider>();
        if (collider != null && !collider.isTrigger)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        int layer = 1 << other.gameObject.layer;

        if ((layer & _playerMask) != 0)
            StartCoroutine(MoneyToPlayerCO(other));


    }

    private void OnTriggerExit(Collider other)
    {
        int layer = 1 << other.gameObject.layer;

        // 초기화 코드
        if((layer & _playerMask) != 0)
        {
          
        }
            
    }

    private IEnumerator MoneyToPlayerCO(Collider playerCollider)
    {
        if (_player == null) yield break;

        PlayerInfo player = _player.GetComponent<PlayerInfo>();
        //List<GameObject> moneyList = GameManager.Instance.Spawn.GetMoney();
        
        if (MoneyList.Count > 0)
        {

            foreach (GameObject money in MoneyList)
            {
                MoneyProjectile projectile = money.GetComponent<MoneyProjectile>();
                MoneyInfo moneyInfo = money.GetComponent<MoneyInfo>();

                if (projectile != null && projectile.gameObject.activeInHierarchy)
                {
                    projectile.enabled = true;

                    Vector3 startPosition = transform.position;
                    Vector3 targetPosition = playerCollider.bounds.center;

                    projectile.Initialize(startPosition, targetPosition, _player.transform, Define.ArriveType.CashType);

                    yield return new WaitUntil(() =>
                    {
                        if (projectile == null) return true;
                        return projectile.HasArrived();
                    });

                    // Money 오브젝트를 제거
                    if (money != null)
                    {
                        if (projectile != null && projectile.gameObject.activeInHierarchy)
                        {
                            player.OnIncome(moneyInfo.Price);
                        }

                        Poolable pool = money.GetComponent<Poolable>();
                        GameManager.Instance.Pool.Push(pool);
                        
                    }
                }
            }
            GameManager.Instance.Tutorial.HandleTriggerEnter(playerCollider, _isCompleteTutorial, Define.NextTutorial.Unlock);
            _isCompleteTutorial = true;
        }


    }
}
