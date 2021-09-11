using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlexFramework.Excel;
using UnityEditor;
using UnityEngine;

public class ExcelImporter : EditorWindow
{
    private static List<string> keyList = new List<string>();
    private static List<List<string>> luaData = new List<List<string>>();
    private static string filePath = "C:\\Users\\ligzh\\Desktop\\ZgameConfig\\SkillCard.xlsx";
    private static int startRow = 1;
    private static int startLine = 0;
    private static string outputFilePath = Application.streamingAssetsPath;
    private static string outputFileName = "skillcard.lua.txt";

#if UNITY_EDITOR
    [MenuItem("Tools/产生lua配置文件")]
#endif
    private static void Init()
    {
        var window = GetWindow<ExcelImporter>("配置文件导入器", true);
    }

    private static void CreatLuaConfigFile()
    {
        keyList = LoadKey(filePath);
        luaData = LoadData(filePath, startRow, startLine);
        string table = CreateLuaTable();
        CreateFile(outputFilePath, outputFileName, table);
    }

    private static void CreateJsonConfigFile()
    {
        
    }

    private static string CreateLuaTable()
    {
        StringBuilder table = new StringBuilder();
        table.Append("-- 工具生成 -- 规则： \n");
        table.Append("-- 每一行为一个数据，下标从 1 - n \n");
        table.Append("-- 每一列对应表中的一个键值对 \n");
        //table.Append("local data = { \n");
        table.Append("data = { \n");
        int index = 1;
        foreach (var dataList in luaData)
        {
            //写入序号
            table.Append("\t [" + index + "] = { \n");
            foreach (var data in dataList)
            {
                table.Append("\t \t" + data + ", \n");
            }

            table.Append("\t }, \n");
            index++;
        }

        table.Append("}\n");
        table.Append("return data\n");
        return table.ToString();
    }


    //读取第一行的key值，存储在一个列表中
    private static List<string> LoadKey(string path, int startRow = 0, int startLine = 0)
    {
        var book = new WorkBook(File.ReadAllBytes(path));
        //将二维数字存到列表，通过行列读取	
        List<Row> rowData = new List<Row>(book[0]);
        List<string> result = new List<string>();
        for (int j = startRow; j < startRow + 1; j++) //读取第一行
        {
            for (int i = startLine; i < rowData[j].Count; i++) //读取所有列的key
            {
                string key = rowData[j][i].Text;
                result.Add(key);
            }
        }

        return result;
    }

    private static List<List<string>> LoadData(string path, int startRow, int startLine)
    {
        var book = new WorkBook(File.ReadAllBytes(path));
        //将二维数字存到列表，通过行列读取	
        List<Row> rowData = new List<Row>(book[0]);
        List<List<string>> result = new List<List<string>>();
        for (int j = startRow; j < rowData.Count; j++) //从第起始行开始读写
        {
            List<string> temp = new List<string>();
            for (int i = startLine; i < rowData[j].Count; i++) //从起始列开始读写
            {
                string str = keyList[i] + " = " + rowData[j][i].Text;
                temp.Add(str);
            }

            result.Add(temp);
        }

        return result;
    }

    private static void CreateFile(string path, string name, string info)
    {
        FileInfo t = new FileInfo(path + "//" + name);
        StreamWriter sw = t.CreateText(); //如果此文件不存在则创建
        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();
    }


    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("配置文件路径:");
        filePath = EditorGUILayout.TextField(filePath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("读取起始行数:");
        startRow = EditorGUILayout.IntField(startRow);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("读取起始列数:");
        startLine = EditorGUILayout.IntField(startLine);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("输出文件路径:");
        outputFilePath = EditorGUILayout.TextField(outputFilePath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("输出文件名:");
        outputFileName = EditorGUILayout.TextField(outputFileName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("生成配置文件"))
        {
            try
            {
                CreatLuaConfigFile();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("通知", "导入失败", "OK");
                throw;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("通知", "导入成功", "OK");
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("打开文件目录"))
        {
            System.Diagnostics.Process.Start(outputFilePath);
        }

        EditorGUILayout.EndHorizontal();
    }
}