using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupDataBase", menuName = "TestProject/PopupDataBase", order = 1)]
public class PopupDataBaseSO : ScriptableObject
{
#pragma warning disable 649

    private static PopupDataBaseSO _instance;
    private static PopupDataBaseSO Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = Resources.Load<PopupDataBaseSO>("PopupDataBase");
            return _instance;
        }
    }
    //public IStorageSO Instance
    //{
    //    get => _instance;
    //    set => _instance = (MobStorageSO)value;
    //}

    [SerializeField]
    private List<UIPopup> _popups;
    public static List<UIPopup> Popups => Instance._popups;

#pragma warning restore 649

    public static UIPopup GetPopup(string name)
    {
        var popup = Popups.FirstOrDefault(i => i.name == name);
        if (popup != null)
            return popup;

        return null;
    }
}