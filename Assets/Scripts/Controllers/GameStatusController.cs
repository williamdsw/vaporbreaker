using UnityEngine;

public class GameStatusController : MonoBehaviour
{
    // Config
    private string nextSceneName;
    private int levelIndex = 0;
    private int newScore = 0;
    private int newTimeScore = 0;
    private int oldScore = 0;
    private int oldTimeScore = 0;
    private bool cameFromLevel = false;
    private bool hasStartedSong = false;
    private bool isLevelCompleted = false;

    //--------------------------------------------------------------------------------//

    // GETTERS / SETTERS

    public string GetNextSceneName () { return nextSceneName; }
    public int GetLevelIndex () { return levelIndex; }
    public int GetNewScore () { return newScore; }
    public int GetNewTimeScore () { return newTimeScore; }
    public int GetOldScore () { return oldScore; }
    public int GetOldTimeScore () { return oldTimeScore; }
    public bool GetCameFromLevel () { return cameFromLevel; }
    public bool GetHasStartedSong () { return hasStartedSong; }
    public bool GetIsLevelCompleted () { return isLevelCompleted; }

    public void SetNextSceneName (string sceneName) { this.nextSceneName = sceneName; }
    public void SetLevelIndex (int levelIndex) { this.levelIndex = levelIndex; }
    public void SetNewScore (int newScore) { this.newScore = newScore; }
    public void SetNewTimeScore (int newTimeScore) { this.newTimeScore = newTimeScore; }
    public void SetOldScore (int oldScore) { this.oldScore = oldScore; }
    public void SetOldTimeScore (int oldTimeScore) { this.oldTimeScore = oldTimeScore; }
    public void SetCameFromLevel (bool cameFromLevel) { this.cameFromLevel = cameFromLevel; }
    public void SetHasStartedSong (bool hasStartedSong) { this.hasStartedSong = hasStartedSong; }
    public void SetIsLevelCompleted (bool isLevelCompleted) { this.isLevelCompleted = isLevelCompleted; }

    //--------------------------------------------------------------------------------//

    private void Awake () 
    {
        SetupSingleton ();
    }

    //--------------------------------------------------------------------------------//

    // Setups a singleton
    private void SetupSingleton ()
    {
        int numberOfInstances = FindObjectsOfType (GetType ()).Length;
        if (numberOfInstances > 1)
        {
            Destroy (this.gameObject);
        }
        else 
        {
            DontDestroyOnLoad (this.gameObject);
        }
    }
}