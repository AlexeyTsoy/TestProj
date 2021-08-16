using UnityEngine;

[CreateAssetMenu(fileName = "SubContainerSO", menuName = "ArcCore/SubContainer", order = 1)]
public class SubContainer : SubContainerSO
{
    protected override void Registration()
    {
        DiService.RegisterInstance<IServiceProvider>(new ServiceProvider(DiContainer));
        DiService.RegisterInstance<IFactoryService<IPopup>>(new PopupFactoryService(DiContainer));
        DiService.Register<GameController>();
        DiService.Register<CommonController>();

        DiService.Register<IPopupService, PopupService>();
        DiService.Register<ILevelService, LevelService>();
        DiService.Register<IGameService, GameService>();
        DiService.Register<IPopupGameOverService, PopupGameOverService>();
    }

    protected override void Resolve()
    {
        DiContainer.Resolve<GameController>();
        DiContainer.Resolve<CommonController>();

        DiContainer.Resolve<ILevelService>();
        DiContainer.Resolve<IGameService>();

        DiContainer.Resolve<IPopupService>();
        DiContainer.Resolve<IPopupGameOverService>();
    }

    protected override void OnSceneLoaded(int scene)
    {
        switch (scene)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                Debug.LogError($"{scene} scene not processed");
                break;
        }
    }

    protected override void OnSceneUnloaded(int scene)
    {
        switch (scene)
        {
            case 0:
                break;
            case 1:
               
                break;
            case 2:
            
                break;
            default:
                Debug.LogError($"{DiContainer} scene not processed");
                break;
        }
    }
}
