using UnityEngine;

public class StartGame : MonoBehaviour
{
    public void StartButton()
    {
        AsyncSceneLoader.Instance.LoadAsyncScene("MainScene");
    }
}
