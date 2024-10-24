using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashTable : Obj_Base
{
    public enum GameObjects
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
    private GameObject _money;

    private bool _isPlayerInArea = false;
    private bool _isCompleteTutorial = false;
    private bool _isFirstNpcToSit = false;

    private int _price;

    private Queue<NPCController> _npcQueue = new Queue<NPCController>();

    NPCController _currentNPC;
    Animator _anim;
    GameObject _player;
    BaseStackMoney _moneyStorageObj;
    PlayerInfo _playerInfo;

    public GameObject GetMoney { get { return _money; }}

    public override void Init()
    {
        _moneyStorageObj = gameObject.GetOrAddComponent<BaseStackMoney>();

        Bind<GameObject>(typeof(GameObjects));

        _playerTrigger = GetObject((int)GameObjects.PlayerTrigger);
        _nPCTrigger = GetObject((int)GameObjects.NPCTrigger);
        _moneyStorage = GetObject((int)GameObjects.MoneyStorage);

        TriggerHandler triggerHandler = _playerTrigger.GetOrAddComponent<TriggerHandler>();
        triggerHandler.Init(this);

        TriggerHandler npcTriggerHandler = _nPCTrigger.GetOrAddComponent<TriggerHandler>();
        npcTriggerHandler.Init(this);

        MoneyTriggerHandler moneyTriggerHandler = _moneyStorage.GetOrAddComponent<MoneyTriggerHandler>();
        moneyTriggerHandler.Init(this);

        _player = GameManager.Instance.Spawn.GetPlayer();
        _playerInfo = _player.GetComponent<PlayerInfo>();

        // ObjectPooling cash
        _money = GameManager.Instance.Resource.Instantiate("Pool/Money", null, transform, 50);
        GameManager.Instance.Resource.Destroy(_money);

    }
    #region TRIGGER EVENT
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
    }

    public void OnPlayerExit(Collider other)
    {
        _isPlayerInArea = false;
    }

    public void OnNPCEnter(Collider other)
    {
        NPCController newNPC = other.GetComponent<NPCController>();

        if (newNPC != null && !_npcQueue.Contains(newNPC))
        {
            _npcQueue.Enqueue(newNPC);
            
        }


        if (newNPC.GetCroassantStackCount() > 0 && newNPC.GetCroassantStackCount() % 2 != 0)
            StartCoroutine(CamActionCo());

        ProcessNextNPC();
    }


    public void OnNPCStay(Collider other){ }

    public void OnNPCExit(Collider other)
    {
        NPCController npc = other.GetComponent<NPCController>();
        npc.AdjustNPCPositions();

        // 돈계산
        _moneyStorageObj.SetKeepMoneyCount(_price);
        _moneyStorageObj.SetParentTransform(_moneyStorage.transform);

        if(npc.GetCroassantStackCount() <= 0)
        {
            Vector3 npcHead = new Vector3(0, 2.0f, 0);

            GameManager.Instance.Sound.Play("cash");
            GameManager.Instance.Particle.Play("VFX_EmojiSmile", npc.transform, npcHead);
        }

        GameManager.Instance.Tutorial.HandleTriggerEnter(other, _isCompleteTutorial, Define.NextTutorial.CashPoint);
        _isCompleteTutorial = true;
    }
    #endregion
    // 재귀
    private void ProcessNextNPC()
    {
        if (_currentNPC == null && _npcQueue.Count > 0)
        {
            NPCController nextNPC = _npcQueue.Dequeue();

            if (nextNPC.GetCroassantStackCount() > 0 && nextNPC.GetCroassantStackCount() % 2 == 0)
            {
                _currentNPC = nextNPC;

                if (_isPlayerInArea && _paperBag == null)
                {
                    StartCoroutine(CreatePaperBagAndProcessNPC());
                }
            }
            else
            {
                // 다시 큐를 처리하여 다음 NPC를 찾음
                ProcessNextNPC();
            }
        }
    }

    #region COROUTINE
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
            HandCroassantInfo handCroassant = croassant.GetComponent<HandCroassantInfo>();

            int numberOfCroassant = _currentNPC.GetCroassantStackCount();

            Vector3 startPosition = croassant.transform.position;
            Vector3 targetPosition = _paperBagSpawnPoint.position + new Vector3(0, 1.5f, 0);
            float duration = 0.1f; // 포물선 비행 시간

            yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            projectile.Initialize(startPosition, targetPosition, _paperBag.transform, Define.ArriveType.NomalType, numberOfCroassant, duration);

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

    private IEnumerator CamActionCo()
    {
        CameraManager.OnChangedCineMachinePriority("Cam_UnLockSit", "Cam_MainCam");

        yield return new WaitForSeconds(2.5f);

        CameraManager.OnChangedCineMachinePriority("Cam_MainCam", "Cam_UnLockSit");
    }
    #endregion
    public void OnPaperBagAnimationComplete()
    {
        StartCoroutine(BreadToPaperBagCo());
    }

}
