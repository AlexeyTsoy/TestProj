using UnityEngine;

public class GameManager : MonoBehaviour
{
#pragma warning disable 0649 // is never assigned to, and will always have its default value null.

    public static GameManager Instance;

    public Puzzle[] puzzles;
    public PuzzleSlot[] puzzleSlots;

    public Transform puzzleParent;

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

    public void BackButton()
    {
        AsyncSceneLoader.Instance.LoadAsyncScene("MainScene");
        GameController.Instance.SetGameState();
    }
}
