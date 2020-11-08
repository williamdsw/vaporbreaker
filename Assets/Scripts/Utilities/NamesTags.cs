
public class NamesTags
{
    // Names
    // PARENTS
    private const string BLOCKS_PARENT_NAME = "Blocks";
    private const string BLOCK_SCORE_TEXT_PARENT_NAME = "Block_Score_Text_Container";
    private const string DEBRIS_PARENT_NAME = "Debris_Container";
    private const string ECHOS_PARENT_NAME = "Echos_Container";
    private const string EXPLOSIONS_PARENT_NAME = "Explosions_Container";
    private const string POWER_UPS_PARENT_NAME = "Power_Ups_Container";
    private const string PROJECTILES_PARENT_NAME = "Projectiles_Container";

    // PANELS
    private const string BINDINGS_PANEL_NAME = "Bindings_Panel";
    private const string DEFAULT_GAMEPAD_LAYOUT_PANEL_NAME = "Default_Gamepad_Layout_Panel";
    private const string DEFAULT_KEYBOARD_LAYOUT_PANEL_NAME = "Default_Keyboard_Layout_Panel";
    private const string MAIN_PANEL_NAME = "Main_Menu_Panel";
    private const string OPTIONS_PANEL_NAME = "Options_Panel";
    private const string PAUSE_PANEL_NAME = "Pause_Panel";
    private const string PROGRESS_PANEL_NAME = "Progress_Panel";
    private const string SELECT_LEVEL_PANEL_NAME = "Select_Level_Panel";

    // OBJECTS
    private const string POWER_UP_ALL_BLOCKS_1_HIT_NAME = "PowerUp_All_Blocks_1_Hit";
    private const string POWER_UP_BALL_BIGGER_NAME = "PowerUp_Ball_Bigger";
    private const string POWER_UP_BALL_FASTER_NAME = "PowerUp_Ball_Faster";
    private const string POWER_UP_BALL_SLOWER_NAME = "PowerUp_Ball_Slower";
    private const string POWER_UP_BALL_SMALLER_NAME = "PowerUp_Ball_Smaller";
    private const string POWER_UP_DUPLICATE_BALL_NAME = "PowerUp_Duplicate_Ball";
    private const string POWER_UP_LEVEL_COMPLETE_NAME = "PowerUp_Level_Complete";
    private const string POWER_UP_PADDLE_EXPAND_NAME = "PowerUp_Paddle_Expand";
    private const string POWER_UP_PADDLE_SHRINK_NAME = "PowerUp_Paddle_Shrink";
    private const string POWER_UP_RESET_BALL_NAME = "PowerUp_Reset_Ball";
    private const string POWER_UP_RESET_PADDLE_NAME = "PowerUp_Reset_Paddle";
    private const string POWER_UP_RANDOM_NAME = "PowerUp_Random";
    private const string POWER_UP_SHOOTER_NAME = "PowerUp_Shooter";
    private const string POWER_UP_UNBREAKABLES_TO_BREAKABLES_NAME = "PowerUp_Unbreakables_To_Breakables";
    private const string POWER_UP_ZERO_DEATHS_NAME = "PowerUp_Zero_Deaths";

    // Tags
    private const string BALL_TAG = "Ball";
    private const string BALL_ECHO_TAG = "Ball_Echo";
    private const string BREAKABLE_BLOCK_TAG = "Breakable";
    private const string LINE_BETWEEN_BALL_POINTER_TAG = "Line_Between_Ball_Pointer";
    private const string PADDLE_TAG = "Paddle";
    private const string PADDLE_ECHO_TAG = "Paddle_Echo";
    private const string POWER_UP_TAG = "PowerUp";
    private const string WALL_TAG = "Wall";
    private const string UNBREAKABLE_BLOCK_TAG = "Unbreakable";

    //--------------------------------------------------------------------------------//
    // GETTERS / SETTERS
    
    // Names

    // PARENTS
    public static string GetBlocksParentName () { return BLOCKS_PARENT_NAME; }
    public static string GetBlockScoreTextParentName () { return BLOCK_SCORE_TEXT_PARENT_NAME; }
    public static string GetDebrisParentName () { return DEBRIS_PARENT_NAME; }
    public static string GetEchosParentName () { return ECHOS_PARENT_NAME; }
    public static string GetExplosionsParentName () { return EXPLOSIONS_PARENT_NAME; }
    public static string GetProjectilesParentName () { return PROJECTILES_PARENT_NAME; }
    public static string GetPowerUpsParentName () { return POWER_UPS_PARENT_NAME; }

    // PANELS
    public static string GetBindingsPanelName () { return BINDINGS_PANEL_NAME; }
    public static string GetDefaultGamepadLayoutPanelName () { return DEFAULT_GAMEPAD_LAYOUT_PANEL_NAME; }
    public static string GetDefaultKeyboardLayoutPanelName () { return DEFAULT_KEYBOARD_LAYOUT_PANEL_NAME; }
    public static string GetMainPanelName () { return MAIN_PANEL_NAME; }
    public static string GetOptionsPanelName () { return OPTIONS_PANEL_NAME; }
    public static string GetPausePanelName () { return PAUSE_PANEL_NAME; }
    public static string GetProgressPanelName () { return PROGRESS_PANEL_NAME; }
    public static string GetSelectLevelPanelName () { return SELECT_LEVEL_PANEL_NAME; }

    // OBJECTS
    public static string GetPowerUpAllBlocks1HitName () { return POWER_UP_ALL_BLOCKS_1_HIT_NAME; }
    public static string GetPowerUpBallBiggerName () { return POWER_UP_BALL_BIGGER_NAME; }
    public static string GetPowerUpBallFasterName () { return POWER_UP_BALL_FASTER_NAME; }
    public static string GetPowerUpBallSlowerName () { return POWER_UP_BALL_SLOWER_NAME; }
    public static string GetPowerUpBallSmallerName () { return POWER_UP_BALL_SMALLER_NAME; }
    public static string GetPowerUpDuplicateBallName () { return POWER_UP_DUPLICATE_BALL_NAME; }
    public static string GetPowerUpLevelCompleteName () { return POWER_UP_LEVEL_COMPLETE_NAME; }
    public static string GetPowerUpPaddleExpandName () { return POWER_UP_PADDLE_EXPAND_NAME; }
    public static string GetPowerUpPaddleShrinkName () { return POWER_UP_PADDLE_SHRINK_NAME; }
    public static string GetPowerUpResetBallName () { return POWER_UP_RESET_BALL_NAME; }
    public static string GetPowerUpResetPaddleName () { return POWER_UP_RESET_PADDLE_NAME; }
    public static string GetPowerUpRandomName () { return POWER_UP_RANDOM_NAME; }
    public static string GetPowerUpShooterName () { return POWER_UP_SHOOTER_NAME; }
    public static string GetPowerUpUnbreakablesToBreakablesName () { return POWER_UP_UNBREAKABLES_TO_BREAKABLES_NAME; }
    public static string GetPowerUpZeroDeathsName () { return POWER_UP_ZERO_DEATHS_NAME; }

    // Tags

    public static string GetBallTag () { return BALL_TAG; }
    public static string GetBallEchoTag () { return BALL_ECHO_TAG; }
    public static string GetBreakableBlockTag () { return BREAKABLE_BLOCK_TAG; }
    public static string GetLineBetweenBallPointerTag () { return LINE_BETWEEN_BALL_POINTER_TAG; }
    public static string GetPaddleTag () { return PADDLE_TAG; }
    public static string GetPaddleEchoTag () { return PADDLE_ECHO_TAG; }
    public static string GetPowerUpTag () { return POWER_UP_TAG; }
    public static string GetUnbreakableBlockTag () { return UNBREAKABLE_BLOCK_TAG; }
    public static string GetWallTag () { return WALL_TAG; }
}