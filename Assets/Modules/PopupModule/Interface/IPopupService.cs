using System;

public interface IPopupService : IDisposable
{
    void ShowPopup(Type type, params object[] args);
    void HidePopup(Type type);

    /// <summary>
    /// Get specify popup
    /// </summary>
    /// <typeparam name="T"> IPopup implementations </typeparam>
    /// <returns></returns>
    T GetPopup<T>();
    /// <summary>
    /// Get base popup
    /// </summary>
    /// <param name="type"> IPopup </param> 
    /// <returns></returns>
    IPopup GetPopup(Type type);
}
