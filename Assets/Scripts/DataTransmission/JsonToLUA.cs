using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System;

public class JsonToLua : MonoBehaviour
{
    //public string jsonName = "emulation_data.json";
    public string luaName = "emulation_data.lua";

    public JSONWriter jsonWriter;

    public string luaData;

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    private float updateInterval = 0.01f;


    void Update()
    {
        timeSinceLastUpdate = Time.time - lastUpdateTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            CustomUpdate();
            lastUpdateTime = Time.time;
        }
    }
    void CustomUpdate()
    {
        string jsonString = jsonWriter.jsonData;
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
        luaData = ConvertToLua(jsonData);
        luaData = ConvertBrackets(luaData);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, luaName), luaData);
    }

    public void ConvertJsonToLua()
    {
        //string jsonPath = Path.Combine(Application.streamingAssetsPath, jsonName);
        //string jsonString = File.ReadAllText(jsonPath);
        string jsonString = jsonWriter.jsonData;
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
        string luaString = ConvertToLua(jsonData);
        luaString = ConvertBrackets(luaString);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, luaName), luaString);
    }

    string ConvertToLua(Dictionary<string, object> data)
    {
        System.Text.StringBuilder luaString = new System.Text.StringBuilder();
        luaString.Append("{");
        foreach (var kvp in data)
        {
            AppendKeyValue(luaString, kvp.Key, kvp.Value, 1);
        }
        luaString.Append("}");
        //luaString.AppendLine("return data");
        return luaString.ToString();
    }

    void AppendKeyValue(System.Text.StringBuilder luaString, string key, object value, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 4);
        if (value is Dictionary<string, object>)
        {
            luaString.Append($"{indent}{key} = {{");
            foreach (var kvp in (Dictionary<string, object>)value)
            {
                AppendKeyValue(luaString, kvp.Key, kvp.Value, indentLevel + 1);
            }
            luaString.Append($"{indent}}},");
        }
        else if (value is IList<object>)
        {
            luaString.Append($"{indent}{key} = {ConvertArrayToLuaTable((IList<object>)value)}");
            luaString.Append(", ");
        }
        else
        {
            luaString.Append($"{indent}{key} = {ConvertValue(value)},");
        }
    }

    string ConvertArrayToLuaTable(IList<object> list)
    {
        System.Text.StringBuilder luaString = new System.Text.StringBuilder();
        luaString.Append("{ ");
        for (int i = 0; i < list.Count; i++)
        {
            AppendValue(luaString, list[i]);
            if (i < list.Count - 1)
            {
                luaString.Append(", ");
            }
        }
        luaString.Append(" }");
        return luaString.ToString();
    }

    void AppendValue(System.Text.StringBuilder luaString, object value)
    {
        if (value is Dictionary<string, object>)
        {
            luaString.Append("{ ");
            foreach (var kvp in (Dictionary<string, object>)value)
            {
                luaString.Append($"{kvp.Key} = ");
                AppendValue(luaString, kvp.Value);
                luaString.Append(", ");
            }
            luaString.Append("}");
        }
        else if (value is IList<object>)
        {
            luaString.Append(ConvertArrayToLuaTable((IList<object>)value));
        }
        else
        {
            luaString.Append(ConvertValue(value));
        }
    }

    string ConvertValue(object value)
    {
        if (value is string)
        {
            return $"\"{value}\"";
        }
        else if (value is bool)
        {
            return value.ToString().ToLower();
        }
        else if (value is float || value is double)
        {
            return ((IFormattable)value).ToString("R", CultureInfo.InvariantCulture);
        }
        else
        {
            return value.ToString();
        }
    }

    string ConvertBrackets(string luaString)
    {
        return Regex.Replace(luaString, @"\[(?!\d)", "{").Replace("]", "}");
    }
}