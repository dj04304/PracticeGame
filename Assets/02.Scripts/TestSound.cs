using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    //TODO Sound삭제용 삭제
    
    //public AudioClip audioClip;
    //public AudioClip audioClip2;

    //int i = 0;
    private void OnTriggerEnter(Collider other)
    {
        //AudioSource audio = GetComponent<AudioSource>();
        //audio.PlayClipAtPoint(audioClip);
        //audio.PlayOneShot(audioClip2);
        //float lifeTime =Mathf.Max(audioClip.length, audioClip2.length);
        //GameObject.Destroy(gameObject, lifeTime);

        //Managers.Sound.Play("UnityChan/univ0001", Define.Sound.Effect);
        //Managers.Sound.Play("UnityChan/univ0002", Define.Sound.Effect);

        //i++;

        //if(i % 2 == 0)
        //    GameManager.Instance.Sound.Play(audioClip, Define.Sound.Bgm);
        //else
        //    GameManager.Instance.Sound.Play(audioClip2, Define.Sound.Bgm);
    }
}
