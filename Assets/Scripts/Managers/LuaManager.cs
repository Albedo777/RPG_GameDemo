using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

/**
 * 基于xlua读取lua配置文件到数据类
 * @author ligzh
 * @date 2021/9/9
 */

public class LuaManager : IClass
{
    Dictionary<int, SkillCardConfig> dictionary;
    private LuaEnv luaEnv;
    private static LuaManager instance;

    public static LuaManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = ClassManager.Instance.Get<LuaManager>();
            }

            return instance;
        }
    }

    public void Init()
    {
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(MyLoader);
    }

    public Dictionary<int, T> LoadConfigFile<T>(string fileName) where T : new()
    {
        luaEnv.DoString(string.Format("require '{0}'", fileName));
        LuaTable tab = luaEnv.Global.Get<LuaTable>("data");
        Dictionary<int, T> dict = new Dictionary<int, T>();
        PreloadConfig(tab, ref dict);
        return dict;
    }

    //加载Lua文件下的.lua.txt 文件
    private byte[] MyLoader(ref string filePath)
    {
        string absPath = Application.dataPath + "/Lua/" + filePath + ".lua.txt";
        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(absPath));
    }

    public static void PreloadConfig<T>(LuaTable table, ref Dictionary<int, T> dictionary, string mainKey = "id")
        where T : new()
    {
        var classType = typeof(T);
        var properties = classType.GetFields();
        for (int i = 1; i <= table.Length; i++)
        {
            var info = table.Get<int, LuaTable>(i);
            if (info != null)
            {
                int id = getInt(info, mainKey);
                T config = new T();
                for (int j = 0; j < properties.Length; j++)
                {
                    var name = properties[j].Name;
                    var type = properties[j].FieldType;
                    var value = GetValue(info, name, type);
                    properties[j].SetValue(config, value);
                }

                dictionary[id] = config;
            }
        }
    }

    private static int getInt(LuaTable table, string key)
    {
        return table.ContainsKey(key) ? table.Get<string, int>(key) : 0;
    }

    private static object GetValue(LuaTable table, string key, Type type)
    {
        if (type == typeof(int))
        {
            return GetValue<int>(table, key);
        }

        if (type == typeof(float))
        {
            return GetValue<float>(table, key);
        }

        if (type == typeof(string))
        {
            return GetValue<string>(table, key);
        }

        var data = table.Get<string, LuaTable>(key);
        if (data == null)
        {
            return default;
        }

        return default;
    }

    private static T GetValue<T>(LuaTable table, string key)
    {
        var type = typeof(T);
        if (table.ContainsKey(key))
        {
            return table.Get<string, T>(key);
        }
        else
        {
            Debug.LogError(key);
            return default;
        }
    }

    public void Update()
    {
        if (luaEnv != null)
        {
            luaEnv.Tick();
        }
    }


    public void OnReset()
    {
        if (luaEnv != null)
        {
            luaEnv.Dispose();
        }
    }
}