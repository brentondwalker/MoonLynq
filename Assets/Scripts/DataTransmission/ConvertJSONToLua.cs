using System;
using System.IO;
using UnityEngine;

public class ConvertJsonToLua : MonoBehaviour
{
    public void WriteLua(string json, string luaName)
    {
        string luaData = ConvertJsonString(json);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, luaName), luaData);
    }

    string ConvertJsonString(string json)
    {
        return json.Replace(":", "=") 
                   .Replace("\"", "")  
                   .Replace("[", "{")  
                   .Replace("]", "}"); 
    }
}