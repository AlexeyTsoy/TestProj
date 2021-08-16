using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompositionRoot : MonoBehaviour
{
#pragma warning disable 0649 // is never assigned to, and will always have its default value null.
    private CompositionRoot[] _objects;

    [SerializeField]
    protected List<SubContainerSO> _subContainers;

    private DIContainer _container;

#pragma warning restore 0649

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _objects = FindObjectsOfType<CompositionRoot>();

        if (_objects.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        InitSystem();

    }
    
    private void Start()
    {
    }
    
    private void InitSystem()
    {
        var diService = new DIService();
        _container = diService.GenerateContainer();
        diService.RegisterInstance<IServiceProvider>(new ServiceProvider(_container));
        
        foreach (var subContainer in _subContainers)
            subContainer.Init(diService, _container);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnApplicationPause(bool pause)
    {
        EventController.Invoke(BaseEventMessage.ON_APPLICATION_PAUSE, pause);
    }

    private void OnApplicationFocus(bool focus)
    {
        EventController.Invoke(BaseEventMessage.ON_APPLICATION_FOCUS, focus);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EventController.Invoke(BaseEventMessage.ON_PRE_LOADED_SCENE, scene.buildIndex);
    }

    private void OnSceneUnloaded(Scene current)
    {
        EventController.Invoke(BaseEventMessage.ON_PRE_UNLOADED_SCENE, current.buildIndex);
        EventController.CheckMonoSubscribers();
    }


    private void OnDestroy()
    {
        if (_objects.Length == 1)
        {
            CoroutineController.StopAllCoroutines();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            EventController.RemoveAll();
            _container.ReleaseAll();
        }
    }
}
