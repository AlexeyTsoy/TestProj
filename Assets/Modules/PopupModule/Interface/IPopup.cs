using System;

public interface IPopup : IDisposable
{
    UIPopup Popup { get; }
    void HidePopup();
    void ShowPopup(params object[] args);
}
