using UnityEngine;

public class LevelService : ILevelService
{
    public LevelService()
    {
        EventController.AddListener<int>(BaseEventMessage.ON_SCENE_LOADED, OnSceneLoaded);
    }

    private void OnSceneLoaded(int scene)
    {
        if (scene == 1)
        {
            OnStartScene();
        }
    }

    private void OnStartScene()
    {
        for (var i = 0; i < 10; i++)
        {
            var obj = Object.Instantiate(LevelManager.Instance.level,
                LevelManager.Instance.levelParent);

            obj.Init(i != 0);
        }
    }


    public void Dispose()
    {
        EventController.RemoveListener<int>(BaseEventMessage.ON_SCENE_LOADED, OnSceneLoaded);
    }
}
