using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerController
{
    private const string CREDITS_SCENE_NAME = "Credits";
    private const string LOADING_SCENE_NAME = "Loading";
    private const string MAIN_MENU_SCENE_NAME = "Main_Menu";
    private const string POWER_UPS_SCENE_NAME = "PowerUps_Guide";
    private const string SELECT_LEVELS_SCENE_NAME = "Select_Levels";
    private const string SOUNDTRACK_SCENE_NAME = "Soundtracks";

    public static string LoadingSceneName => "Loading";
    public static string MainMenuSceneName => "Main_Menu";
    public static string SelectLevelsSceneName => "Select_Levels";
    public static string TitleSceneName => "Title";
    


    public static string GetCreditsSceneName()
    {
        return CREDITS_SCENE_NAME;
    }

    public static string GetLoadingSceneName()
    {
        return LOADING_SCENE_NAME;
    }

    public static string GetMainMenuSceneName()
    {
        return MAIN_MENU_SCENE_NAME;
    }
    public static string GetPowerUpsSceneName()
    {
        return POWER_UPS_SCENE_NAME;
    }

    public static string GetSelectLevelsSceneName()
    {
        return SELECT_LEVELS_SCENE_NAME;
    }

    public static string GetSoundtrackSceneName()
    {
        return SOUNDTRACK_SCENE_NAME;
    }

    /// <summary>
    /// Calls a scene by name
    /// </summary>
    /// <param name="sceneName"> Valid scene name </param>
    public static void CallScene(string sceneName) => SceneManager.LoadScene(sceneName);

    /// <summary>
    /// Calls a scene asynchronously by name
    /// </summary>
    /// <param name="sceneName"> Valid scene name </param>
    public static AsyncOperation CallSceneAsync(string sceneName) => SceneManager.LoadSceneAsync(sceneName);

    /// <summary>
    /// Get build index of active scene
    /// </summary>
    public static int GetActiveSceneIndex() => SceneManager.GetActiveScene().buildIndex;

    /// <summary>
    /// Quits the application
    /// </summary>
    public static void QuitGame() => Application.Quit();

    /// <summary>
    /// Reload actual scene
    /// </summary>
    public static void ReloadScene() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
}