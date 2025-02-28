using System;
using System.IO;
using UnityEngine;
public class LocalFileHandler : FileHandler
{
    public override void Load(string path, Action<string> onLoad)
    {
        string result = null;
        if (File.Exists(path))
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"path: {path}, Error: {e}");
            }
        }
        onLoad?.Invoke(result);
    }

    public override void Save(string path, string content, Action onSave)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        try
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"path: {path}, Error: {e}");
        }
        onSave?.Invoke();
    }
}