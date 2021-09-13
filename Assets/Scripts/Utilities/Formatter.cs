﻿using System;

namespace Utilities
{
    public class Formatter
    {
        public static string FormatEllapsedTimeInHours(int timer)
        {
            int hours = timer / 3600;
            int minutes = (timer - (hours * 3600)) / 60;
            int seconds = timer - (hours * 3600) - (minutes * 60);
            return string.Concat(hours.ToString("00"), ":", minutes.ToString("00"), ":", seconds.ToString("00"));
        }

        public static string FormatEllapsedTimeInMinutes(int timer)
        {
            int hours = timer / 3600;
            int minutes = (timer - (hours * 3600)) / 60;
            int seconds = timer - (hours * 3600) - (minutes * 60);
            return string.Format("{0}:{1}", minutes.ToString("00"), seconds.ToString("00"));
        }

        public static string FormatLevelName(string levelName)
        {
            // Cancels
            if (string.IsNullOrEmpty(levelName) || string.IsNullOrWhiteSpace(levelName)) return string.Empty;
            string newName = levelName.Split(new[] { "__" }, StringSplitOptions.None)[1];
            newName = newName.Replace("_", " ");
            return newName;
        }

        public static string FormatToCurrency(long value) => value.ToString("#,###,###,###");
    }
}