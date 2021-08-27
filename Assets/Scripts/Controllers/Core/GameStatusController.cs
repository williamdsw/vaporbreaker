using UnityEngine;

public class GameStatusController : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private int levelIndex = 0;
    [SerializeField] private long levelId = 0;
    [SerializeField] private long newScore = 0;
    [SerializeField] private long newTimeScore = 0;
    [SerializeField] private long oldScore = 0;
    [SerializeField] private long oldTimeScore = 0;
    [SerializeField] private bool cameFromLevel = false;
    [SerializeField] private bool hasStartedSong = false;
    [SerializeField] private bool isLevelCompleted = false;

    public string GetNextSceneName()
    {
        return nextSceneName;
    }

    public int GetLevelIndex()
    {
        return levelIndex;
    }

    public long GetNewScore()
    {
        return newScore;
    }

    public long GetNewTimeScore()
    {
        return newTimeScore;
    }

    public long GetOldScore()
    {
        return oldScore;
    }

    public long GetOldTimeScore()
    {
        return oldTimeScore;
    }

    public bool GetCameFromLevel()
    {
        return cameFromLevel;
    }

    public bool GetHasStartedSong()
    {
        return hasStartedSong;
    }

    public bool GetIsLevelCompleted()
    {
        return isLevelCompleted;
    }

    public void SetNextSceneName(string sceneName)
    {
        this.nextSceneName = sceneName;
    }

    public void SetLevelIndex(int levelIndex)
    {
        this.levelIndex = levelIndex;
    }

    public void SetNewScore(int newScore)
    {
        this.newScore = newScore;
    }

    public void SetNewTimeScore(int newTimeScore)
    {
        this.newTimeScore = newTimeScore;
    }

    public void SetOldScore(int oldScore)
    {
        this.oldScore = oldScore;
    }

    public void SetOldTimeScore(int oldTimeScore)
    {
        this.oldTimeScore = oldTimeScore;
    }

    public void SetCameFromLevel(bool cameFromLevel)
    {
        this.cameFromLevel = cameFromLevel;
    }

    public void SetHasStartedSong(bool hasStartedSong)
    {
        this.hasStartedSong = hasStartedSong;
    }

    public void SetIsLevelCompleted(bool isLevelCompleted)
    {
        this.isLevelCompleted = isLevelCompleted;
    }


    // || Properties

    public static GameStatusController Instance { get; private set; }

    public string NextSceneName { get => nextSceneName; set => nextSceneName = value; }
    public int LevelIndex { get => levelIndex; set => levelIndex = value; }
    public long LevelId { get => levelId; set => levelId = value; }
    public long NewScore { get => newScore; set => newScore = value; }
    public long NewTimeScore { get => newTimeScore; set => newTimeScore = value; }
    public long OldScore { get => oldScore; set => oldScore = value; }
    public long OldTimeScore { get => oldTimeScore; set => oldTimeScore = value; }
    public bool CameFromLevel { get => cameFromLevel; set => cameFromLevel = value; }
    public bool HasStartedSong { get => hasStartedSong; set => hasStartedSong = value; }
    public bool IsLevelCompleted { get => isLevelCompleted; set => isLevelCompleted = value; }


    private void Awake() => SetupSingleton();

    private void SetupSingleton()
    {
        int numberOfInstances = FindObjectsOfType(GetType()).Length;
        if (numberOfInstances > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}