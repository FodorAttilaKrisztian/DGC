using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;  
using System.IO;
using System.Text;

public class FileDataHandler
{
    private readonly string dataDirPath;
    private readonly string dataFileName;
    private readonly bool useEncryption;
    private readonly string encryptionCodeWord = "peanutbutter";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log("Error occured when trying to load data from file: " + fullPath + "\n" + e);
                loadedData = new GameData();
            }
        }

        return loadedData;
    }

    public void Save(GameData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Attempted to save null GameData.");
            return;
        }

        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        
            string dataToStore = JsonUtility.ToJson(data, true);

            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    private string EncryptDecrypt(string data)
    {
        var modifiedData = new StringBuilder(data.Length);

        for (int i = 0; i < data.Length; i++)
        {
            modifiedData.Append((char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]));
        }

        return modifiedData.ToString();
    }
}