﻿using System;

namespace Utilities
{
    [Serializable]
    public class PlayerProgress
    {
        // || State

        private int currentLevelIndex;
        private bool hasPlayerFinishedGame = false;

        // || Properties

        public int CurrentLevelIndex { get => currentLevelIndex; set => currentLevelIndex = value; }
        public bool HasPlayerFinishedGame { get => hasPlayerFinishedGame; set => hasPlayerFinishedGame = value; }

        public PlayerProgress()
        {
            CurrentLevelIndex = 0;
            HasPlayerFinishedGame = false;
        }
    }
}