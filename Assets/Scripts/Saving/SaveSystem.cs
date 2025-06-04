using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
      public static void SaveSoundData(AudioManager audioManager)
      {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/sound.data";
            FileStream stream = new FileStream(path, FileMode.Create);

            SoundData soundData = new SoundData(audioManager);

            formatter.Serialize(stream, soundData); 
            stream.Close(); 
      }
        
    public static void SaveGameData(int skullCount)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath +"/game.data";

        FileStream stream = new FileStream(path, FileMode.Create);

        GameData gameData = new GameData(skullCount);
        formatter.Serialize(stream, gameData);
        stream.Close();
    }
    public static SoundData LoadSoundData()
    {
        string path = Application.persistentDataPath + "/sound.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SoundData soundData = formatter.Deserialize(stream) as SoundData;
            stream.Close();
            return soundData;
            

        }
        else
        {
            Debug.LogError("Save Data file not found in "+ path);
            return null;
        }
    }

    public static GameData LoadGameData()
    {
        string path = Application.persistentDataPath + "/game.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData gameData = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return gameData;


        }
        else
        {
            Debug.LogError("Game Data file not found in " + path);
            return null;
        }
    }


}
