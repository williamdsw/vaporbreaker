using UnityEngine;

public class PersistentData : MonoBehaviour
{
    //State 
    private int startingSceneIndex;

    private void Awake()
    {
        SetupSingleton();
    }

    private void Start()
    {
        startingSceneIndex = SceneManagerController.GetActiveSceneIndex();
    }

    private void Update()
    {
        int currentSceneIndex = SceneManagerController.GetActiveSceneIndex();
        if (currentSceneIndex != startingSceneIndex)
        {
            Destroy(this.gameObject);
        }
    }

    // Define singleton
    private void SetupSingleton()
    {
        int numberOfInstances = FindObjectsOfType(GetType()).Length;
        if (numberOfInstances > 1)
        {
            DestroyImmediate(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}