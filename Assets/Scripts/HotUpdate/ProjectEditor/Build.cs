using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using System.Data.SqlTypes;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class Build 
{
    //buildPath = 根路径+Builds/Game.exe
    private static string buildPath => Path.Combine(new DirectoryInfo(Application.dataPath).Parent.FullName,"Builds/Game.exe");

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
        //注意dllName已经自带了后缀.dll
        foreach (string dllName in dllConfig.aot)
        {
            string dllPath = Path.Combine(aotDllDir, $"{dllName}");
            string dllBytesPath = Path.Combine(aotTextDir, $"{dllName}.bytes");

            if (!File.Exists(dllPath)) dllPath = Path.Combine(hotUpdateDllDir, $"{dllName}");// 万一AOT目录中没有，则用HotUpdate中的

            File.Copy(dllPath, dllBytesPath, true);
            AssetDatabase.Refresh();
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID($"Assets/HotUpdate/DllBytes/AOT/{dllName}.bytes"), aotGroup);
            //entry.SetAddress(dllName+".dll"); 这个和下面这个写法最终结果都是一样的，文件名称.dll
            entry.SetAddress($"{dllName}");
        }

        //HotUpdate
        AddressableAssetGroup hotUpdateGroup = settings.FindGroup("HotUpdate");
        foreach (string dllName in dllConfig.hotUpdate)
        {
            string dllPath = Path.Combine(hotUpdateDllDir, $"{dllName}");
            string dllBytesPath = Path.Combine(hotUpdateTextDir, $"{dllName}.bytes");

            File.Copy(dllPath, dllBytesPath, true);
            AssetDatabase.Refresh();
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID($"Assets/HotUpdate/DllBytes/HotUpdate/{dllName}.bytes"), hotUpdateGroup);
            //entry.SetAddress(dllName+".dll"); 这个和下面这个写法最终结果都是一样的，文件名称.dll
            entry.SetAddress($"{dllName}");
        }

        //PriorityHotUpdate
        AddressableAssetGroup priorityHotUpdateGroup = settings.FindGroup("PriorityHotUpdate");
        foreach (string dllName in dllConfig.priorityHotUpdate)
        {
            //PriorityHotUpdate的dll的路径就是放在HotUpdate里的，所以这个不变。但是文本text的路径是PriorityHotUpdate自己的
            string dllPath = Path.Combine(hotUpdateDllDir, $"{dllName}");
            string dllBytesPath = Path.Combine(priorityHotUpdateTextDir, $"{dllName}.bytes");

            File.Copy(dllPath, dllBytesPath, true);
            AssetDatabase.Refresh();
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID($"Assets/HotUpdate/DllBytes/PriorityHotUpdate/{dllName}.bytes"), priorityHotUpdateGroup);
            //entry.SetAddress(dllName+".dll"); 这个和下面这个写法最终结果都是一样的，文件名称.dll
            entry.SetAddress($"{dllName}");
        }

        //做完上面步骤，需要标记，代表被修改过的
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("成功生成dll文本文件");
    }


    /// <summary>
    /// 客户端构建
    /// </summary>
    [MenuItem("Build/NewClient")]
    public static void NewClient()
    {
        //调用HybridCLR的生成命令
        PrebuildCommand.GenerateAll();
        //生成并搬迁dll文件,同时设置到Addressables中去
        GenerateDllFiles();
        //构建场景
        string[] scenes = new string[EditorSceneManager.sceneCountInBuildSettings];
        for(int i = 0; i< EditorSceneManager.sceneCountInBuildSettings; i++)
        {
            scenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
            //获取到Scene是有几种不同的api都能获得的，获得的内容多少有不同处：对比SceneManager和EditorSceneManager的api
            Debug.Log($"添加场景:{scenes[i]}");
        }
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            target = EditorUserBuildSettings.activeBuildTarget,
            locationPathName = buildPath
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log("完成客户端构建。构建路径buildPath:"+ buildPath);
    }
}
