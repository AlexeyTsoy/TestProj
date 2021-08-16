using System;

public class CommonController : IDisposable
{
    public static CommonController Instance { get; private set; }
    
    private readonly IServiceProvider _serviceProvider;
    
    public CommonController(IServiceProvider serviceProvider)
    {
        Instance = this;
        _serviceProvider = serviceProvider;
    }

    #region PopupService
    public void ShowPopup(Type type, params object[] args)
    {
        _serviceProvider.GetService<IPopupService>().ShowPopup(type, args);
    }

    public void HidePopup(Type type)
    {
        _serviceProvider.GetService<IPopupService>().HidePopup(type);
    }

    public T GetPopup<T>()
    {
        return _serviceProvider.GetService<IPopupService>().GetPopup<T>();
    }
    #endregion

    public void Dispose()
    {
        Instance = null;
    }
}
