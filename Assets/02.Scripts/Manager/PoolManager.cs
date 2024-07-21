using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PoolManager : MonoBehaviour
{
    #region Pool
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Queue<Poolable> _poolQueue = new Queue<Poolable>();

        public void Init(GameObject original, int count = 50)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"@{original.name}_Root";

            for (int i = 0; i < count; i++)
                Push(Create());
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;

            return go.GetOrAddComponent<Poolable>();
        }

        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.transform.localPosition = Vector3.zero;
            poolable.transform.localRotation = Quaternion.identity;
            poolable.gameObject.SetActive(false);
            poolable.IsUsing = false;

            _poolQueue.Enqueue(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if (_poolQueue.Count > 0)
                poolable = _poolQueue.Dequeue();
            else
                poolable = Create();

            poolable.gameObject.SetActive(true);

            poolable.transform.parent = parent;
            poolable.IsUsing = true;

            return poolable;
        }

    }
    #endregion

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    public Transform Root { get; set; }

    public void Init()
    {
        Root = gameObject.transform;
    }

    public void CreatePool(GameObject original, Transform spawnPos, int count = 50)
    {
        Pool pool = new Pool();
        pool.Init(original, count);

        if(spawnPos != null)
        {
            pool.Root.parent = spawnPos;
            pool.Root.position = spawnPos.position;
        }
        else
            pool.Root.parent = Root.transform;

        _pool.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if(_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null, Transform spawnPos = null, int count = 50)
    {
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original, spawnPos, count);


        return _pool[original.name].Pop(parent);
    }

    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;

        return _pool[name].Original;
    }


}
