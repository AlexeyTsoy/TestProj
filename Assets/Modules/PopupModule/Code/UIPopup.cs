using UnityEngine;

public abstract class UIPopup : MonoBehaviour
{
    private GameObject _popup;
    public void Hide()
    {
        Destroy(_popup);
    }

    public void Show()
    {
        var parent = GameObject.FindGameObjectWithTag("PopupCanvas");
        _popup = Instantiate(gameObject, parent.transform);
    }
}
