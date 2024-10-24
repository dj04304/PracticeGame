using UnityEngine;

public class CashTableStackMoney : BaseStackMoney
{

    GameObject _player;
    PlayerInfo _playerInfo;
    
    protected override void Init()
    {
        _player = GameManager.Instance.Spawn.GetPlayer();
        _playerInfo = _player.GetComponent<PlayerInfo>();
        _cashTable = GetComponent<CashTable>();
    }

    protected override void Update()
    {
        base.Update();
    }

}
