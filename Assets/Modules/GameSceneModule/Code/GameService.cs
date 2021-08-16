using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Newtonsoft.Json;

public class GameService : IGameService
{
    private readonly IPopupService _popupService;

    public GameService(IPopupService popupService)
    {
        _popupService = popupService;
        EventController.AddListener<int>(BaseEventMessage.ON_SCENE_LOADED, OnSceneLoaded);
        EventController.AddListener<PointerEventData, PuzzleSlot>(EventMessage.ON_DROP, OnDrop);
    }

    private void OnSceneLoaded(int scene)
    {
        if (scene == 2)
        {
            OnStartScene();
        }
    }

    private void OnStartScene()
    {
        GetGameState();

        for (var i = 0; i < GameManager.Instance.puzzleSlots.Length; i++)
        {
            if (GameManager.Instance.puzzleSlots[i].puzzleSlot.state == 0)
            {
                var puzzle = GameManager.Instance.puzzles.FirstOrDefault(p =>
                    p.type == GameManager.Instance.puzzleSlots[i].puzzleSlot.type);
                if (puzzle != null)
                {
                    Object.Instantiate(puzzle, GameManager.Instance.puzzleParent);
                }
            }
            else
            {
                GameManager.Instance.puzzleSlots[i].SetSlot();
            }
        }
    }

    private static void GetGameState()
    {
        if (PlayerPrefs.HasKey(Prefs.GAME_STATE))
        {
            var slots = JsonConvert.DeserializeObject<PuzzleSlotModel[]>(PlayerPrefs.GetString(Prefs.GAME_STATE));
            for (var i = 0; i < GameManager.Instance.puzzleSlots.Length; i++)
            {
                var slot = slots.FirstOrDefault(s => s.type == GameManager.Instance.puzzleSlots[i].puzzleSlot.type);
                if (slot != null)
                {
                    GameManager.Instance.puzzleSlots[i].puzzleSlot = slot;
                }
            }
        }
        else
        {
            for (var i = 0; i < GameManager.Instance.puzzleSlots.Length; i++)
            {
                GameManager.Instance.puzzleSlots[i].puzzleSlot.state = 0;
            }
        }
    }

    private void OnDrop(PointerEventData eventData, PuzzleSlot puzzleSlot)
    {
        if (eventData == null || eventData.pointerDrag == null) return;
        var puzzle = eventData.pointerDrag.GetComponent<Puzzle>();
        if (puzzle != null && puzzle.type == puzzleSlot.puzzleSlot.type && puzzleSlot.puzzleSlot.state == 0)
        {
            puzzleSlot.puzzleSlot.state = 1;
            puzzleSlot.SetSlot();
            Object.Destroy(eventData.pointerDrag.gameObject);
            CheckGameState();
        }
    }

    private void CheckGameState()
    {
        var count = 0;
        for (var i = 0; i < GameManager.Instance.puzzleSlots.Length; i++)
        {
            if (GameManager.Instance.puzzleSlots[i].puzzleSlot.state == 1)
                count++;
            else
                return;
        }

        if (GameManager.Instance.puzzleSlots.Length != count) return;
        GameOver();
    }

    private void GameOver()
    {
        //GameOver
        PlayerPrefs.DeleteKey(Prefs.GAME_STATE);
        _popupService.ShowPopup(typeof(IPopupGameOverService));
    }

    public void SetGameState()
    {
        var slots = new PuzzleSlotModel[GameManager.Instance.puzzleSlots.Length];

        for (var i = 0; i < GameManager.Instance.puzzleSlots.Length; i++)
        {
            slots[i] = GameManager.Instance.puzzleSlots[i].puzzleSlot;
        }

        var json = JsonConvert.SerializeObject(slots);
        PlayerPrefs.SetString(Prefs.GAME_STATE, json);
    }

    public void Dispose()
    {
        PlayerPrefs.DeleteKey(Prefs.GAME_STATE);

        EventController.RemoveListener<int>(BaseEventMessage.ON_SCENE_LOADED, OnSceneLoaded);
        EventController.AddListener<PointerEventData, PuzzleSlot>(EventMessage.ON_DROP, OnDrop);
    }
}
