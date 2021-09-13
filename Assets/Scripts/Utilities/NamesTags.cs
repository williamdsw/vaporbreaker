
namespace Utilities
{
    public class NamesTags
    {
        // PARENTS
        public static string BlocksParentName => "Blocks";
        public static string BlockScoreTextParentName => "Block_Score_Text_Container";
        public static string DebrisParentName => "Debris_Container";
        public static string EchosParentName => "Echos_Container";
        public static string ExplosionsParentName => "Explosions_Container";
        public static string PowerUpsParentName => "Power_Ups_Container";
        public static string ProjectilesParentName => "Projectiles_Container";

        public class Blocks
        {
            public static string PowerUpBlock => "PowerUpBlock";
        }
        public class Functions
        {
            public static string UndoFireBalls => "UndoFireBalls";
        }

        public class Panels
        {
            public static string LanguagePanelName => "LanguagePanel";
            public static string MainPanelName => "MainMenuPanel";
            public static string OptionsPanelName => "OptionsPanel";
            public static string PausePanelName => "Pause_Panel";
            public static string ProgressPanelName => "ProgressPanel";
            public static string SelectLevelPanelName => "Select_Level_Panel";
        }




        // Tags
        public static string BallTag => "Ball";
        public static string BallEchoTag => "Ball_Echo";
        public static string BreakableBlockTag => "Breakable";
        public static string LineBetweenBallPointerTag => "Line_Between_Ball_Pointer";
        public static string PaddleTag => "Paddle";
        public static string PaddleEchoTag => "Paddle_Echo";
        public static string PowerUpTag => "PowerUp";
        public static string WallTag => "Wall";
        public static string UnbreakableBlockTag => "Unbreakable";
    }
}