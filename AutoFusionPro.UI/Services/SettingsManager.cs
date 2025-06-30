using AutoFusionPro.Core.Configuration;
using AutoFusionPro.Core.Enums.SystemEnum;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoFusionPro.UI.Services
{
    public static class SettingsManager
    {
        private static readonly string SettingsFilePath = "appsettings.json";

        public static AppSettings LoadSettings()
        {
            // Check if the settings file exists
            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    return JsonSerializer.Deserialize<AppSettings>(json,
                        new JsonSerializerOptions
                        {
                            Converters = { new JsonStringEnumConverter() }
                        }) ?? CreateDefaultSettings();
                }
                catch (JsonException ex)
                {
                    // Log the error
                    Console.WriteLine($"Error loading settings: {ex.Message}");

                    // If there's an error, backup the current file and create new default settings
                    if (File.Exists(SettingsFilePath))
                    {
                        string backupPath = $"{SettingsFilePath}.bak";
                        try
                        {
                            File.Copy(SettingsFilePath, backupPath, true);
                            Console.WriteLine($"Backed up existing settings to {backupPath}");
                        }
                        catch
                        {
                            // Ignore backup errors
                        }
                    }

                    return CreateAndSaveDefaultSettings();
                }
            }

            // If file does not exist, create it with default settings
            return CreateAndSaveDefaultSettings();
        }

        public static void SaveSettings(AppSettings settings)
        {
            // Serialize the settings to JSON and write to the file
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            });
            File.WriteAllText(SettingsFilePath, json);
        }

        public static void RestoreDefaultSettings()
        {
            // Create and save default settings
            var defaultSettings = CreateDefaultSettings();
            SaveSettings(defaultSettings);
        }

        private static AppSettings CreateDefaultSettings()
        {
            // Create default settings with StartDate set to current date
            return new AppSettings
            {
                Language = Languages.Arabic,
                IsDarkThemeEnabled = false,
                SystemName = "AutoFusion Pro",
                LogoPath = "",
                SelectedCurrency = Currency.USD // Add default currency

            };
        }

        private static AppSettings CreateAndSaveDefaultSettings()
        {
            // Create default settings and save to file
            var defaultSettings = CreateDefaultSettings();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }
    }
}
