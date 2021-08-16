using UnityEngine;


/// <summary>
/// Add attribute
///[CreateAssetMenu(fileName = "SubContainerSO", menuName = "Core/ContainerName", order = 1)]
/// </summary>
public abstract class SubContainerSO : ScriptableObject
{
    protected virtual void Awake()
    {
    }
    
    protected DIService DiService { get; set; }
    protected DIContainer DiContainer { get; set; }

    protected virtual void OnEnable()
    {
        EventController.AddListener<int>(BaseEventMessage.ON_PRE_LOADED_SCENE, OnPreloadedScene);
        EventController.AddListener<int>(BaseEventMessage.ON_PRE_UNLOADED_SCENE, OnPreUnloadedScene);
    }

    protected virtual void OnDisable()
    {
        EventController.RemoveListener<int>(BaseEventMessage.ON_PRE_LOADED_SCENE, OnPreloadedScene);
        EventController.RemoveListener<int>(BaseEventMessage.ON_PRE_UNLOADED_SCENE, OnPreUnloadedScene);
    }

    protected virtual void OnDestroy()
    {
    }

    private void OnPreloadedScene(int scene)
    {
        OnSceneLoaded(scene);
        EventController.Invoke(BaseEventMessage.ON_SCENE_LOADED, scene);
    }

    private void OnPreUnloadedScene(int scene)
    {
        OnSceneUnloaded(scene);
        EventController.Invoke(BaseEventMessage.ON_SCENE_UNLOADED, scene);
    }

    protected virtual void OnSceneLoaded(int scene)
    {

    }

    protected virtual void OnSceneUnloaded(int scene)
    {

    }

    public void Init(DIService diService, DIContainer container)
    {
        DiService = diService;
        DiContainer = container;
        Registration();
        Resolve();
    }

    //EXAMPLES
    //DiService.RegisterInstance<IFactoryService<ISomeService>>(new SomeService(container));
    //DiService.RegisterInstance(new LazyService<ISomeService>(container));
    //DiService.Register<ISomeService, SomeService>(ServiceLifeTime.Transient);
    //DiService.Register<ISomeService, SomeService>();
    //DiService.Register<SomeService>();
    protected abstract void Registration();

    protected abstract void Resolve();
}
