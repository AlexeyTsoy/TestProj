using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private void Awake()
    {
        var objects = FindObjectsOfType<CoroutineManager>();

        if (objects.Length > 2)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }
}