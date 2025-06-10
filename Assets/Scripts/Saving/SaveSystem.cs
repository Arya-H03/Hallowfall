using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;

public static class SaveSystem
{
    private static string gamePath = Application.persistentDataPath + "/game.json";
    private static string settingsPath = Application.persistentDataPath + "/settings.json";

    private static void SaveGameData(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(gamePath, json);
    }
    private static void SaveSettingsData(SettingsData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(settingsPath, json);
    }

    public static GameData LoadGameData()
    {
        if (File.Exists(gamePath))
        {
            string json = File.ReadAllText(gamePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            Debug.LogWarning("No save file found at " + gamePath);
            return new GameData(); 
        }
    }

    public static SettingsData LoadSettingsData()
    {
        if (File.Exists(settingsPath))
        {
            string json = File.ReadAllText(settingsPath);
            return JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            Debug.LogWarning("No save file found at " + settingsPath);
            return new SettingsData();
        }
    }
    
    public static void UpdatePlayerSkulls(int newAmount)
    {
        GameData data = LoadGameData();
        if (data == null) { data = new GameData(); }

        data.skullCount = newAmount;
        SaveGameData(data);
    }

    public static void UpdateAudioSettings(AudioManager audioManager)
    {
        SettingsData data = LoadSettingsData();
        if (data == null) { data = new SettingsData(); }
        data.masterVolume = audioManager.MasterVolumeMultiplier;
        data.effectsVolume = audioManager.EffectsVolumeMultiplier;
        data.musicVolume = audioManager.MusicVolumeMultiplier;

        SaveSettingsData(data);

 
    }

    public static void UpdateSkillTree(int skillID, bool isUnlocked)
    {
        GameData gameData = LoadGameData();
        if (gameData == null) { gameData = new GameData(); }

        gameData.skillTreeNodes[skillID] = isUnlocked ? 1 : 0;
        SaveGameData(gameData);
            
    }
}
