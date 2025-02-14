using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class ConvertJsonToLua : MonoBehaviour
{
    public void WriteLua(string json, string luaName)
    {
        string luaData = ConvertToLua(json);
        luaData = ConvertBrackets(luaData);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, luaName), luaData);
    }

    string ConvertToLua(string json)
    {
        object jsonData = JsonConvert.DeserializeObject<object>(json);
        return ConvertObjectToLua(jsonData, 0);
    }

    string ConvertObjectToLua(object obj, int indentLevel)
    {
        StringBuilder luaString = new StringBuilder();
        string indent = new string(' ', indentLevel * 4);

        if (obj is IList<object> list)
        {
            luaString.Append("{\n");
            foreach (var item in list)
            {
                luaString.Append(indent + "    " + ConvertObjectToLua(item, indentLevel + 1) + ",\n");
            }
            luaString.Append(indent + "}");
        }
        else if (obj is Dictionary<string, object> dict) 
        {
            luaString.Append("{\n");
            foreach (var kvp in dict)
            {
                luaString.Append(indent + "    " + kvp.Key + " = " + ConvertObjectToLua(kvp.Value, indentLevel + 1) + ",\n");
            }
            luaString.Append(indent + "}");
        }
        else if (obj is string str) 
        {
            luaString.Append($"\"{str}\"");
        }
        else if (obj is bool boolean) 
        {
            luaString.Append(boolean.ToString().ToLower());
        }
        else if (obj is float || obj is double) 
        {
            luaString.Append(((IFormattable)obj).ToString("R", CultureInfo.InvariantCulture));
        }
        else 
        {
            luaString.Append(obj.ToString());
        }

        

        return luaString.ToString();
    }

    string ConvertBrackets(string luaString)
    {
        return luaString.Replace("[", "{").Replace("]", "}");
    }
}