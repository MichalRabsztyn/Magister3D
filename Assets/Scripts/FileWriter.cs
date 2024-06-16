using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileWriter : MonoBehaviour
{
    #region Instance
    private static FileWriter m_Instance;
    public static FileWriter Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<FileWriter>();
                if (m_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(FileWriter).Name;
                    m_Instance = obj.AddComponent<FileWriter>();
                }
            }
            return m_Instance;
        }
    }
    #endregion

    public string fileName = "WyslijMnie.dat";
    public string path = "./";

    private string filePath;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        filePath = path + fileName;
    }

    public void WriteValuesToFile(float value1, float value2, float value3)
    {
        string line = $"{value1},{value2},{value3}";

        if (!File.Exists(filePath))
        {
            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.WriteLine(line);
            }
        }
        else
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(line);
            }
        }
    }
    
    public int GetLastAttemptIndex()
    {
        if (!File.Exists(filePath))
        {
            return 0;
        }

        int index = 0;
        using (StreamReader reader = File.OpenText(filePath))
        {
            while (!reader.EndOfStream)
            {
                reader.ReadLine();
                index++;
            }
        }

        return index;
    }
}
