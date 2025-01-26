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
            Debug.LogError("Save file not found in "+ path);
            return null;
        }
    }

    
}
