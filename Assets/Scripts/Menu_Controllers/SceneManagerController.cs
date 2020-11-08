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
    private const string TITLE_SCENE_NAME = "Title_Screen";

    //--------------------------------------------------------------------------------//
    // GETTERS

    public static string GetCreditsSceneName () { return CREDITS_SCENE_NAME; }
    public static string GetLoadingSceneName () { return LOADING_SCENE_NAME; }
    public static string GetMainMenuSceneName () { return MAIN_MENU_SCENE_NAME; }
    public static string GetPowerUpsSceneName () { return POWER_UPS_SCENE_NAME; }
    public static string GetSelectLevelsSceneName () { return SELECT_LEVELS_SCENE_NAME; }
    public static string GetSoundtrackSceneName () { return SOUNDTRACK_SCENE_NAME; }
    public static string GetTitleSceneName () { return TITLE_SCENE_NAME; }

    //--------------------------------------------------------------------------------//

    // Calls scene by name
    public static void CallScene (string sceneName)
    {
        SceneManager.LoadScene (sceneName);
    }

    // Calls scene async by name
    public static AsyncOperation CallSceneAsync (string sceneName)
    {
        return SceneManager.LoadSceneAsync (sceneName);
    }

    // Get active scene's index
    public static int GetActiveSceneIndex ()
    {
        return SceneManager.GetActiveScene ().buildIndex;
    }

    // Go to next scene
    public static void GotoNextScene ()
    {
        int sceneIndex = SceneManager.GetActiveScene ().buildIndex + 1;
        SceneManager.LoadSceneAsync (sceneIndex);
    }

    // Quits the application
    public static void QuitGame ()
    {
        Application.Quit ();
    }
    
    // Reloads the actual scene
    public static void ReloadScene ()
    {
        int sceneIndex = SceneManager.GetActiveScene ().buildIndex;
        SceneManager.LoadSceneAsync (sceneIndex);
    }
}