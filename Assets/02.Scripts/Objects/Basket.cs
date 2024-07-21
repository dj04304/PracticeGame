using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{

    private int _mask = (1 << (int)Define.Layer.Player);

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if(playerController != null && playerController.GetCroassantStackCount() > 0) 
            {
                Debug.Log("½ÇÇà");
            }
        }
    }

}
