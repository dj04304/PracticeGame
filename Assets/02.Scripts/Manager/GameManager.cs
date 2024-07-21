using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    private DataManager _data = new DataManager();
    private InputManager _input = new InputManager();
    private PoolManager _pool;
    private ResourceManager _resource = new ResourceManager();
    private SoundManager _sound = new SoundManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private SpawnManager _spawn = new SpawnManager();
    private UIManager _ui = new UIManager();
    private Util _util = new Util();

    public DataManager Data {  get { return _data; } }
    public InputManager Input { get { return _input; } }
    public PoolManager Pool { get { return _pool; } }
    public ResourceManager Resource { get { return _resource; } }
    public SoundManager Sound { get { return _sound; } }
    public SceneManagerEx Scene { get { return _scene; } }
    public SpawnManager Spawn { get { return _spawn; } }
    public UIManager UI { get { return _ui; } }
    public Util Util { get { return _util; } }

    protected override void Awake()
    {
        base.Awake();

        EnsureManager(ref _pool, gameObject.transform);
        //_poolManager = FindObjectOfType<ObjectPoolManager>();

        Init();
    }

    private void EnsureManager<T>(ref T manager, Transform parent) where T : Component
    {
        manager = FindObjectOfType<T>();
        if (manager == null)
        {
            GameObject managerObject = new GameObject(typeof(T).Name);
            managerObject.transform.SetParent(parent);
            manager = managerObject.AddComponent<T>();
        }
    }

    private void Init()
    {
        _data.Init();
        _sound.Init();
        _pool.Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    public void Clear() 
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
    }

}
