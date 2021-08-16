using UnityEngine;

public class PopupGameOver : UIPopup
{
#pragma warning disable 0649 // is never assigned to, and will always have its default value null.

#pragma warning restore 0649

    public void BackButton()
    {
        AsyncSceneLoader.Instance.LoadAsyncScene("MainScene");
        HidePopup();
    }

    public void RetryButton()
    {
        AsyncSceneLoader.Instance.LoadAsyncScene("GameScene");
        HidePopup();
    }

    private void HidePopup()
    {
        CommonController.Instance.HidePopup(typeof(IPopupGameOverService));
    }

    private void OnDestroy()
    {
        HidePopup();
    }

}
