using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace WindOS.System
{
    public static class ConfigManager
    {
        private static string configPath = @"0:\settings.cfg";
        private static Dictionary<string, string> settings = new Dictionary<string, string>();

        // Defaults
        public static Color ThemeColor = Color.Teal;
        public static Color TaskbarColor = Color.Navy;
        public static string PIN = "1234";
        public static int WallpaperIndex = 0;
        public static int ScreenTimeout = 300; // Seconds
        public static string Language = "English";

        public static void Load()
        {
            settings.Clear();
            if (File.Exists(configPath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(configPath);
                    foreach (var line in lines)
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            settings[parts[0].Trim()] = parts[1].Trim();
                        }
                    }
                    ApplySettings();
                }
                catch { }
            }
        }

        public static void Save()
        {
            // Update dictionary from current static values
            settings["ThemeColor"] = ThemeColor.ToArgb().ToString();
            settings["TaskbarColor"] = TaskbarColor.ToArgb().ToString();
            settings["PIN"] = PIN;
            settings["WallpaperIndex"] = WallpaperIndex.ToString();
            settings["ScreenTimeout"] = ScreenTimeout.ToString();
            settings["Language"] = Language;

            List<string> lines = new List<string>();
            foreach (var kvp in settings)
            {
                lines.Add($"{kvp.Key}={kvp.Value}");
            }

            try
            {
                File.WriteAllLines(configPath, lines.ToArray());
            }
            catch { }
        }

        private static void ApplySettings()
        {
            if (settings.ContainsKey("ThemeColor")) ThemeColor = Color.FromArgb(int.Parse(settings["ThemeColor"]));
            if (settings.ContainsKey("TaskbarColor")) TaskbarColor = Color.FromArgb(int.Parse(settings["TaskbarColor"]));
            if (settings.ContainsKey("PIN")) PIN = settings["PIN"];
            if (settings.ContainsKey("WallpaperIndex")) WallpaperIndex = int.Parse(settings["WallpaperIndex"]);
            if (settings.ContainsKey("ScreenTimeout")) ScreenTimeout = int.Parse(settings["ScreenTimeout"]);
            if (settings.ContainsKey("Language")) Language = settings["Language"];
        }
    }
}
