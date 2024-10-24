using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{
    #region Managers
    private InputManager _input = new InputManager();
    private ObjectManager _object = new ObjectManager();
    private PoolManager _pool;
    private ParticleManager _particle;
    private ResourceManager _resource = new ResourceManager();
    private SoundManager _sound = new SoundManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private SpawnManager _spawn = new SpawnManager();
    private UIManager _ui = new UIManager();
    private Util _util = new Util();
    private TutorialManager _tutorial = new TutorialManager();
    private GameScene _game;

    public InputManager Input { get { return _input; } }
    public ObjectManager Object { get { return _object; } }
    public PoolManager Pool { get { return _pool; } }
    public ParticleManager Particle { get { return _particle; } }
    public ResourceManager Resource { get { return _resource; } }
    public SoundManager Sound { get { return _sound; } }
    public SceneManagerEx Scene { get { return _scene; } }
    public SpawnManager Spawn { get { return _spawn; } }
    public UIManager UI { get { return _ui; } }
    public Util Util { get { return _util; } }
    public TutorialManager Tutorial { get { return _tutorial; } }
    public GameScene Game { get { return _game; } }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        EnsureManager(ref _pool, gameObject.transform);
        EnsureManager(ref _particle, gameObject.transform);
        //_poolManager = FindObjectOfType<ObjectPoolManager>();

        Init();
    }

    private void Init()
    {
        _sound.Init();
        _pool.Init();
        _tutorial.Init();
    }
    private void EnsureManager<T>(ref T manager, Transform parent) where T : Component
    {
        manager = FindObjectOfType<T>();
        if (manager == null)
        {
            GameObject managerObject = new GameObject($"@{typeof(T).Name}");
            managerObject.transform.SetParent(parent);
            manager = managerObject.AddComponent<T>();
        }
    }

    void Update()
    {
        //_input.OnUpdate();
    }

    public void Clear() 
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
    }

}
