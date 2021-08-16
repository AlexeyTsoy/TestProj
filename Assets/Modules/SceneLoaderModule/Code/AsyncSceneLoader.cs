using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncSceneLoader : MonoBehaviour
{
    public static AsyncSceneLoader Instance { get; private set; }

    public Slider slider;

    public TextMeshProUGUI progressText;

    public GameObject panel;


    private void Awake()
    {
        var objects = FindObjectsOfType<AsyncSceneLoader>();

        if (objects.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
    }
    
    public void LoadAsyncScene(string sceneName, Action callback = null)
    {
        //if (SceneManager.GetActiveScene().name == sceneName) return;
        CoroutineController.StartCoroutine(LoadAsyncSceneRoutine(sceneName, callback));
    }
    
    private IEnumerator LoadAsyncSceneRoutine(string sceneName, Action callback)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        var operationProgress = operation.progress;
        
        if (panel != null)
            panel.SetActive(true);
        if (slider != null)
            slider.value = 0;
        //progressText.text = "0%";

        while (operationProgress < 1)
        {
            operationProgress = operation.progress;
            var progress = Mathf.Clamp01(operationProgress / 0.9f);
            if (slider != null)
                slider.value = progress;
            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
                    
            yield return null;
        }

        if (panel != null)
            panel.SetActive(false);

        callback?.Invoke();
    }

    private void OnDestroy()
    {
    }

    private void OnApplicationQuit()
    {
        
    }
}
