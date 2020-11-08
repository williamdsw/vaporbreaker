using System;
using System.Collections.Generic;

[Serializable]
public class PlayerProgress
{
    // Data
    private static int totalNumberOfLevels = 56;
    private int currentLevelIndex;
    private List<string> levelNamesList = new List<string> ();
    private List<bool> isLevelUnlockedList = new List<bool> ();
    private List<bool> isLevelCompletedList = new List<bool> ();
    private List<int> highScoresList = new List<int> ();
    private List<int> highTimeScoresList = new List<int> ();
    private List<DateTime?> lastTimePlayedList = new List<DateTime?> ();

    //--------------------------------------------------------------------------------//
    // GETTERS / SETTERS

    public int GetCurrentLevelIndex () { return currentLevelIndex; }
    public List<int> GetHighScoresList () { return highScoresList; }
    public List<int> GetHighTimeScoresList () { return highTimeScoresList; }
    public List<bool> GetIsLevelCompletedList () { return isLevelCompletedList; }
    public List<bool> GetIsLevelUnlockedList () { return isLevelUnlockedList; }
    public List<DateTime?> GetLastTimePlayedList () { return lastTimePlayedList; }
    public List<string> GetLevelNamesList () { return levelNamesList; }

    public void SetCurrentLevelIndex (int currentLevelIndex) { this.currentLevelIndex = currentLevelIndex; }
    public void SetHighScoresList (List<int> highScoresList) { this.highScoresList = highScoresList; }
    public void SetHighTimeScoresList (List<int> highTimeScoresList) { this.highTimeScoresList = highTimeScoresList; }
    public void SetIsLevelCompletedList (List<bool> isLevelCompletedList) { this.isLevelCompletedList = isLevelCompletedList; }
    public void SetIsLevelUnlockedList (List<bool> isLevelUnlockedList) { this.isLevelUnlockedList = isLevelUnlockedList; }
    public void SetLastTimePlayedList (List<DateTime?> lastTimePlayedList) { this.lastTimePlayedList = lastTimePlayedList; }

    //--------------------------------------------------------------------------------//

    // Constructor with default values
    public PlayerProgress ()
    {
        currentLevelIndex = 0;

        for (int i = 0; i < totalNumberOfLevels; i++)
        {
            isLevelUnlockedList.Add ((i == 0 ? true : false));
            isLevelCompletedList.Add (false);
            highScoresList.Add (0);
            highTimeScoresList.Add (0);
            lastTimePlayedList.Add (null);
        }

        FillLevelNamesList ();
    }

    //--------------------------------------------------------------------------------//

    private void FillLevelNamesList ()
    {
        // Normal
        levelNamesList.Clear ();
        levelNamesList.Add ("Level_01__1_Hit_Blocks");
        levelNamesList.Add ("Level_02__2_Hits_Blocks");
        levelNamesList.Add ("Level_03__3_Hits_Blocks");
        levelNamesList.Add ("Level_04__4_Hits_Blocks");
        levelNamesList.Add ("Level_05__5_Hits_Blocks");
        levelNamesList.Add ("Level_06__Random_Hits_Blocks");
        levelNamesList.Add ("Level_07__PowerUp_Blocks");
        levelNamesList.Add ("Level_08__Unbreakables_Blocks");
        levelNamesList.Add ("Level_09__Burning");
        levelNamesList.Add ("Level_10__Bluemerize");
        levelNamesList.Add ("Level_11__RGB");
        levelNamesList.Add ("Level_12__Big_1_Hit_R");
        levelNamesList.Add ("Level_13__Big_1_Hit_G");
        levelNamesList.Add ("Level_14__Big_1_Hit_B");
        levelNamesList.Add ("Level_15__Green_XX");
        levelNamesList.Add ("Level_16__Purple_I");
        levelNamesList.Add ("Level_17__Brown_n_Butter");
        levelNamesList.Add ("Level_18__Big_2_Hits_R");
        levelNamesList.Add ("Level_19__Big_2_Hits_G");
        levelNamesList.Add ("Level_20__Big_2_Hits_B");
        levelNamesList.Add ("Level_21__Gray_V");
        levelNamesList.Add ("Level_22__Hit");
        levelNamesList.Add ("Level_23__Tetris_1");
        levelNamesList.Add ("Level_24__Big_3_Hits_R");
        levelNamesList.Add ("Level_25__Big_3_Hits_G");
        levelNamesList.Add ("Level_26__Big_3_Hits_B");
        levelNamesList.Add ("Level_27__Bricks_1");
        levelNamesList.Add ("Level_28__Bricks_2");
        levelNamesList.Add ("Level_29__Bricks_3");
        levelNamesList.Add ("Level_30__Big_4_Hits_R");
        levelNamesList.Add ("Level_31__Big_4_Hits_G");
        levelNamesList.Add ("Level_32__Big_4_Hits_B");
        levelNamesList.Add ("Level_33__Cant_Die_1");
        levelNamesList.Add ("Level_34__Cant_Die_2");
        levelNamesList.Add ("Level_35__Cant_Die_3");
        levelNamesList.Add ("Level_36__Big_5_Hits_R");
        levelNamesList.Add ("Level_37__Big_5_Hits_G");
        levelNamesList.Add ("Level_38__Big_5_Hits_B");
        levelNamesList.Add ("Level_39__Squares");
        levelNamesList.Add ("Level_40__Triangles");
        levelNamesList.Add ("Level_41__The_Big_Pentagon");
        levelNamesList.Add ("Level_42__Big_PowerUp_Block_R");
        levelNamesList.Add ("Level_43__Big_PowerUp_Block_G");
        levelNamesList.Add ("Level_44__Big_PowerUp_Block_B");
        levelNamesList.Add ("Level_45__Blue_Squares");
        levelNamesList.Add ("Level_46__Big_Blue_Gradient_Broken_Square");
        levelNamesList.Add ("Level_47__Blue_Chained_Blocks");
        levelNamesList.Add ("Level_48__Big_Random_Hit_Block_R");
        levelNamesList.Add ("Level_49__Big_Random_Hit_Block_G");
        levelNamesList.Add ("Level_50__Big_Random_Hit_Block_B");
        levelNamesList.Add ("Level_51__Trapeze");
        levelNamesList.Add ("Level_52__Hexagon");
        levelNamesList.Add ("Level_53__Cross");
        levelNamesList.Add ("Level_54__Big_Unbreakable_Block_R");
        levelNamesList.Add ("Level_55__Big_Unbreakable_Block_G");
        levelNamesList.Add ("Level_56__Big_Unbreakable_Block_B");
    }
}