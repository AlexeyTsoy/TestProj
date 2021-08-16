using UnityEngine;

public class PopupGameOverService : IPopupGameOverService
{
    public UIPopup Popup { get; set; }

    public PopupGameOverService()
    {
        
    }

    public void Dispose()
    {
    }

    public void HidePopup()
    {
        if (Popup != null)
        {
            Popup.Hide();
        }
        Popup = null;
    }

    public void ShowPopup(params object[] args)
    {
        if (Popup != null) return;
        Popup = PopupDataBaseSO.GetPopup("PopupGameOver");
        if (Popup != null)
            Popup.Show();
    }
}
