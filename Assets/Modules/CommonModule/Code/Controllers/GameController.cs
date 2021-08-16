using System;

public class GameController : IDisposable
{
    public static GameController Instance { get; private set; }
    
    private readonly IServiceProvider _serviceProvider;
    
    public GameController(IServiceProvider serviceProvider)
    {
        Instance = this;
        _serviceProvider = serviceProvider;
    }

    #region GameSceneService

    public void SetGameState()
    {
        _serviceProvider.GetService<IGameService>().SetGameState();
    }

    #endregion

    public void Dispose()
    {
        Instance = null;
    }
}
