using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockPlane : LockObj_Base
{
    public static event Action OnUnlockEvent;
    enum GameObjects
    {
        UI_Lock,
        UI_Price,
        Price,
    }

    private GameObject _uiLock;
    private GameObject _uiPrice;
    private TMP_Text _priceText;

    [SerializeField] private int _priceValue = 14;

    [SerializeField] private Vector3 _nextLockPosition = Vector3.zero;
    [SerializeField] private Quaternion _nextLockRotation = Quaternion.identity;
    [SerializeField] private Vector3 _nextLockScale = Vector3.one;

    private bool _isCompleteTutorial = false;

    private int _mask = (1 << (int)Define.Layer.Player);

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        _uiLock = GetObject((int)GameObjects.UI_Lock);
        _uiPrice = GetObject((int)GameObjects.UI_Price);
        _priceText = GetObject((int)GameObjects.Price)?.GetComponent<TMP_Text>();

        if (_priceText != null)
            _priceText.text = _priceValue.ToString();


    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            PlayerInfo player = other.GetComponent<PlayerInfo>();
            TryUnlock(player);
        }
    }

    public override bool CanUnlock(PlayerInfo player)
    {
        if (player == null)
        {
            return false;
        }

        bool canUnlock = player.Money >= _priceValue;

        if (!canUnlock)
            Debug.Log($"Not enough money: ({player.Money}), price: ({_priceValue}).");

        return canUnlock;
    }

    public void TryUnlock(PlayerInfo player)
    {
        if (CanUnlock(player))
        {
            Unlock(player);
        }
    }

    protected override void OnUnlock(PlayerInfo player)
    {
        SpawnMoney();
    }

    private void CompleteUnlock()
    {
        gameObject.SetActive(false);
        Sit sit = GameManager.Instance.Object.GetObj<Sit>();
        sit.gameObject.SetActive(true);
        GameManager.Instance.Sound.Play("Success");
        GameManager.Instance.Particle.Play("VFX_AppearSignStand", sit.transform, Vector3.zero, Define.Particle.Effect);

        OnUnlockEvent?.Invoke();

        // 위치, 회전, 스케일 설정
        Vector3 nextPosition = SetNextLockPos();
        Quaternion nextRotation = SetNextLockRot();
        Vector3 nextScale = SetNextLockScale();

        UI_Price uiPrice = GameManager.Instance.UI.MakeWorldSpaceUI<UI_Price>(null, nextPosition, nextRotation, nextScale);
        GameManager.Instance.Tutorial.AddUnlockPoint(uiPrice.transform);
        _isCompleteTutorial = true;
    }


    private void SpawnMoney()
    {
        GameObject go = new GameObject { name = "@Money" };
        PlayerMoneyPool pool = go.GetOrAddComponent<PlayerMoneyPool>();

        pool.SetParentTransform(transform);
        pool.AddAcumulateCount(_priceValue);
        pool.OnDecreaseMoney += UpdateUI;
    }


    protected override Vector3 SetNextLockPos() { return _nextLockPosition; }
    protected override Quaternion SetNextLockRot() { return _nextLockRotation; }
    protected override Vector3 SetNextLockScale() { return _nextLockScale; }

    private void UpdateUI(int decreaseMoney)
    {
        _priceText.text = $"{_priceValue -= decreaseMoney}";

        if(_priceValue <= 0)
            CompleteUnlock(); ;
    }
}

