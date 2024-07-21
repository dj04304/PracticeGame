using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public void Init()
    {
        // TODO : Define���� �����丵, ��� �̸� ����
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = GameManager.Instance.Resource.Load<TextAsset>($"Data/{path}");

        //return JsonUtility.FromJson<Loader>(textAsset.text);
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
