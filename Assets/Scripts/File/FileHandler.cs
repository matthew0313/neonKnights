using System;
using System.Collections;
using System.Collections.Generic;
public abstract class FileHandler
{
    public abstract void Load(string path, Action<string> onLoad);
    public abstract void Save(string path, string content, Action onSave);
}