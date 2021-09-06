using System.IO;
using UnityEngine;

namespace MVC.Global
{
    public class Configuration
    {
        public class Queries
        {
            public class Level
            {
                public static string GetById => string.Concat(ListAll, " WHERE id = {0} ");
                public static string GetLastLevel => string.Concat(ListAll, " WHERE id = (SELECT MAX(id) FROM level) ");
                public static string ListAll => " SELECT id, name, is_unlocked, is_completed FROM level ";
                public static string ResetLevels => " UPDATE level SET is_unlocked = 0, is_completed = 0; UPDATE level SET is_unlocked = 1 WHERE id = (SELECT MIN(id) AS id FROM level); ";
                public static string UpdateFieldById => " UPDATE level {0} WHERE id = {1}; ";
            }

            public class Localization
            {
                public static string GetByLanguage => " SELECT id, language, content FROM localization WHERE language = '{0}'; ";
                public static string GetLanguages => " SELECT id, language, content FROM localization ";
            }

            public class Scoreboard
            {
                public static string DeleteAll => " DELETE FROM scoreboard; ";
                public static string GetByMaxScoreByLevel => " SELECT id, level_id, MAX(score) as score, MIN(time_score) as time_score, MAX(best_combo) as best_combo, moment FROM scoreboard WHERE level_id = {0}; ";
                public static string Insert => " INSERT INTO scoreboard (level_id, score, time_score, best_combo, moment) VALUES ({0}, {1}, {2}, {3}, {4}); ";
                public static string ListByLevel => " SELECT id, level_id, score, time_score, best_combo, moment FROM scoreboard WHERE level_id = {0} ORDER BY score DESC; ";
            }

            public class Track
            {
                public static string ListAll => " SELECT * FROM track ";
            }
        }

        public class Properties
        {
            public static string DatabaseName => "database.s3db";
            public static string DatabasePath => string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);
            public static string DatabaseStreamingAssetsPath => string.Format("{0}/StreamingAssets/{1}", Application.dataPath, DatabaseName);
            public static string ProgressPath => Path.Combine(Application.persistentDataPath, "SaveProgress.dat");
        }

        public class InputsNames
        {
            public static string ChangeSong => "Change_Song";
            public static string Pause => "Pause";
            public static string Shoot => "Shoot";
            public static string UiCancel => "UI_Cancel";
            public static string UiLeft => "UI_Left";
            public static string UiRight => "UI_Right";
            public static string UiSubmit => "UI_Submit";
        }
    }
}