using System;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    private CashTable _cashTable;

    private int _playerMask = (1 << (int)Define.Layer.Player);
    private int _npcMask = (1 << (int)Define.Layer.NPC);

    public void Init(CashTable cashTable)
    {
        _cashTable = cashTable;
    }

    private void HandleTrigger(Collider other, Action<Collider> playerAction, Action<Collider> npcAction)
    {
        int layer = 1 << other.gameObject.layer;
        if ((_playerMask & layer) != 0)
        {
            playerAction(other);
        }
        else if ((_npcMask & layer) != 0)
        {
            npcAction(other);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other, _cashTable.OnPlayerEnter, _cashTable.OnNPCEnter);
    }

    private void OnTriggerStay(Collider other)
    {
        HandleTrigger(other, _cashTable.OnPlayerStay, _cashTable.OnNPCStay);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleTrigger(other, _cashTable.OnPlayerExit, _cashTable.OnNPCExit);
    }
}