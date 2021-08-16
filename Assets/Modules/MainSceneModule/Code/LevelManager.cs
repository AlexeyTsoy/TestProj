using UnityEngine;

public class LevelManager : MonoBehaviour
{
#pragma warning disable 0649 // is never assigned to, and will always have its default value null.

    public static LevelManager Instance;

    public Level level;

    public Transform levelParent;

#pragma warning restore 0649

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
