using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using HybridCLR.Editor;
using System.Data.SqlTypes;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

public static class Build 
{
    [MenuItem("Build/GenerateDllFiles")]
    public static void GenerateDllFiles()
    {
        Debug.Log("开始生成dll文本文件");
        string environmentDir = Environment.CurrentDirectory;
        string aotDllDir = Path.Combine(environmentDir, SettingsUtil.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget));
        string hotUpdateDllDir = Path.Combine(environmentDir, SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget));
       
        string aotTextDir = Path.Combine(environmentDir, "Assets/HotUpdate/DllBytes/AOT");
        string hotUpdateTextDir = Path.Combine(environmentDir, "Assets/HotUpdate/DllBytes/HotUpdate");
        string priorityHotUpdateTextDir = Path.Combine(environmentDir, "Assets/HotUpdate/DllBytes/PriorityHotUpdate");
        
        DllConfig dllConfig = AssetDatabase.LoadAssetAtPath<DllConfig>("Assets/HotUpdate/HotUpdateConfig/DllConfig.asset");
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;


        //AOT
        AddressableAssetGroup aotGroup = settings.FindGroup("AOT");
        foreach(string dllName in dllConfig.aot)
        {
            string dllPath = Path.Combine(aotDllDir, $"{dllName}.dll");
            string dllBytesPath = Path.Combine(aotTextDir, $"{dllName}.dll.bytes");

            if (!File.Exists(dllPath)) dllPath = Path.Combine(hotUpdateDllDir, $"{dllName}.dll");// 万一AOT目录中没有，则用HotUpdate中的

            File.Copy(dllPath, dllBytesPath, true);
            AssetDatabase.Refresh();
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID($"Assets/HotUpdate/DllBytes/AOT/{dllName}.dll.bytes"), aotGroup);
            //entry.SetAddress(dllName+".dll"); 这个和下面这个写法最终结果都是一样的，文件名称.dll
            entry.SetAddress($"{dllName}.dll");
        }

        //HotUpdate
        AddressableAssetGroup hotUpdateGroup = settings.FindGroup("HotUpdate");
        foreach (string dllName in dllConfig.hotUpdate)
        {
            string dllPath = Path.Combine(hotUpdateDllDir, $"{dllName}.dll");
            string dllBytesPath = Path.Combine(hotUpdateTextDir, $"{dllName}.dll.bytes");

            File.Copy(dllPath, dllBytesPath, true);
            AssetDatabase.Refresh();
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID($"Assets/HotUpdate/DllBytes/HotUpdate/{dllName}.dll.bytes"), hotUpdateGroup);
            //entry.SetAddress(dllName+".dll"); 这个和下面这个写法最终结果都是一样的，文件名称.dll
            entry.SetAddress($"{dllName}.dll");
        }

        //PriorityHotUpdate
        AddressableAssetGroup priorityHotUpdateGroup = settings.FindGroup("PriorityHotUpdate");
        foreach (string dllName in dllConfig.priorityHotUpdate)
        {
            //PriorityHotUpdate的dll的路径就是放在HotUpdate里的，所以这个不变。但是文本text的路径是PriorityHotUpdate自己的
            string dllPath = Path.Combine(hotUpdateDllDir, $"{dllName}.dll");
            string dllBytesPath = Path.Combine(priorityHotUpdateTextDir, $"{dllName}.dll.bytes");

            File.Copy(dllPath, dllBytesPath, true);
            AssetDatabase.Refresh();
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID($"Assets/HotUpdate/DllBytes/PriorityHotUpdate/{dllName}.dll.bytes"), priorityHotUpdateGroup);
            //entry.SetAddress(dllName+".dll"); 这个和下面这个写法最终结果都是一样的，文件名称.dll
            entry.SetAddress($"{dllName}.dll");
        }



        //做完上面步骤，需要标记，代表被修改过的
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("成功生成dll文本文件");
    }
}
