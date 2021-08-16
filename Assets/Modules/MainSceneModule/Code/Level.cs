using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
#pragma warning disable 0649 // is never assigned to, and will always have its default value null.
    [SerializeField]
    private GameObject _lock;

    [SerializeField]
    private Button _startLevelButton;
#pragma warning restore 0649

    public void Init(bool state)
    {
        if (!state)
        {
            _startLevelButton.onClick.AddListener(() => AsyncSceneLoader.Instance.LoadAsyncScene("GameScene"));
            _lock.SetActive(false);
        }
        else
        {
            _startLevelButton.enabled = false;
            _lock.SetActive(true);
        }
    }
}
