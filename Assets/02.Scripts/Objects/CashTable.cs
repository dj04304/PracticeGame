using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashTable : Obj_Base
{
    enum GameObjects
    {
        PlayerTrigger,
        NPCTrigger,
        MoneyStorage,
    }

    [SerializeField]
    private Transform _paperBagSpawnPoint;

    private GameObject _playerTrigger;
    private GameObject _nPCTrigger;
    private GameObject _moneyStorage;

    private GameObject _paperBag;
    private bool _isPlayerInArea = false;
    private int _price;

    private NPCController _currentNPC;
    private Animator _anim;

    GameObject _player;
    PlayerInfo _playerInfo;
    MoneySpawningPool _moneyPool;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        _playerTrigger = GetObject((int)GameObjects.PlayerTrigger);
        _nPCTrigger = GetObject((int)GameObjects.NPCTrigger);
        _moneyStorage = GetObject((int)GameObjects.MoneyStorage);

        TriggerHandler triggerHandler = _playerTrigger.GetOrAddComponent<TriggerHandler>();
        triggerHandler.Init(this);

        TriggerHandler npcTriggerHandler = _nPCTrigger.GetOrAddComponent<TriggerHandler>();
        npcTriggerHandler.Init(this);

        _player = GameManager.Instance.Spawn.GetPlayer();
        _playerInfo = _player.GetComponent<PlayerInfo>();

        _moneyPool = gameObject.GetOrAddComponent<MoneySpawningPool>();
        _moneyPool.SetParentTransform(_moneyStorage.transform);
    }

    public void OnPlayerEnter(Collider other)
    {
        _isPlayerInArea = true;

        if (_currentNPC != null && _paperBag == null)
        {
            StartCoroutine(CreatePaperBagAndProcessNPC());
        }
    }

    public void OnPlayerStay(Collider other)
    {
        Debug.Log("PlayerStay");
    }

    public void OnPlayerExit(Collider other)
    {
        Debug.Log("PlayerExit");
        _isPlayerInArea = false;
    }

    public void OnNPCEnter(Collider other)
    {
        if (_currentNPC == null)
        {
            _currentNPC = other.GetComponent<NPCController>();
            if (_isPlayerInArea && _paperBag == null)
            {
                StartCoroutine(CreatePaperBagAndProcessNPC());
            }
        }
    }

    public void OnNPCStay(Collider other)
    {
        Debug.Log("NPCStay");
    }

    public void OnNPCExit(Collider other)
    {
        Debug.Log("NPCExit");
        NPCController npc = other.GetComponent<NPCController>();
        npc.AdjustNPCPositions();
        // 돈계산
        _moneyPool.SetKeepMoneyCount(_price);

    }

    private IEnumerator CreatePaperBagAndProcessNPC()
    {
        GameObject go = GameManager.Instance.Resource.Instantiate("World/PaperBag", _paperBagSpawnPoint);
        if (go != null)
        {
            _paperBag = go;
            _anim = _paperBag.GetComponent<Animator>();

            if (_anim != null)
            {
                yield return new WaitForSeconds(1.5f);
                OnPaperBagAnimationComplete();
            }
            else
            {
                OnPaperBagAnimationComplete();
            }
        }
        else
        {
            OnPaperBagAnimationComplete();
        }
    }

    private IEnumerator BreadToPaperBagCo()
    {
        if (_currentNPC == null) yield break;

        int croassantCount = _currentNPC.GetCroassantStackCount();

        while (croassantCount > 0)
        {
            GameObject croassant = _currentNPC.RemoveToCroassantStack();
            if (croassant == null)
            {
                yield break;
            }

            CroassantProjectile projectile = croassant.GetComponent<CroassantProjectile>();
            HandCroassant handCroassant = croassant.GetComponent<HandCroassant>();

            int numberOfCroassant = _currentNPC.GetCroassantStackCount();

            Vector3 startPosition = croassant.transform.position;
            Vector3 targetPosition = _paperBagSpawnPoint.position + new Vector3(0, 1.5f, 0);
            float duration = 0.1f; // 포물선 비행 시간

            yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            projectile.Initialize(startPosition, targetPosition, _paperBag.transform, Define.ArriveType.CashType, numberOfCroassant, duration);

            yield return new WaitUntil(() => projectile.HasArrived());

            GameManager.Instance.Spawn.Despawn(croassant);

            _price += handCroassant.Price;

            croassantCount--;
        }

        if (croassantCount == 0)
        {
            yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            if (_anim != null)
                _anim.SetBool("IsFull", true);

            yield return new WaitForSeconds(1.5f);

            StartCoroutine(PaperBagToNPC());
        }
    }

    private IEnumerator PaperBagToNPC()
    {
        if (_currentNPC == null) yield break;

        _currentNPC.AddToPaperBagStack(_paperBag);

        PaperBagProjectile projectile = _paperBag.GetComponent<PaperBagProjectile>();

        Vector3 startPosition = _paperBagSpawnPoint.position;
        Vector3 targetPosition = _currentNPC.HandPos.position;

        projectile.Initialize(startPosition, targetPosition, _currentNPC.HandPos, Define.ArriveType.BagType);

        yield return new WaitUntil(() => projectile.HasArrived());

        _currentNPC.StartMovingToEntrance();

        _paperBag = null;
        _currentNPC = null;
    }

    public void OnPaperBagAnimationComplete()
    {
        StartCoroutine(BreadToPaperBagCo());
    }
}
