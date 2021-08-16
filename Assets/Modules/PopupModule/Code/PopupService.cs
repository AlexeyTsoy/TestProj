using System;

public class PopupService : IPopupService
{
    private readonly IFactoryService<IPopup> _factoryService;

    public PopupService(IFactoryService<IPopup> factoryService)
    {
        _factoryService = factoryService;
    }

    public void ShowPopup(Type type, params object[] args)
    {
        _factoryService.GetService(type).ShowPopup(args);
    }

    public void HidePopup(Type type)
    {
        _factoryService.GetService(type).HidePopup();

    }

    /// <summary>
    /// Get specify popup
    /// </summary>
    /// <typeparam name="T"> IPopup implementations </typeparam>
    /// <returns></returns>
    public T GetPopup<T>()
    {
        return (T)_factoryService.GetService(typeof(T));
    }

    /// <summary>
    /// Get base popup
    /// </summary>
    /// <param name="type"> IPopup </param> 
    /// <returns></returns>
    public IPopup GetPopup(Type type)
    {
        return _factoryService.GetService(type);
    }

    public void Dispose()
    {
    }
}