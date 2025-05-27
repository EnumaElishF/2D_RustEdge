using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    int sceneIndex = -1;
    GUIContent[] sceneNames;
    readonly string[] scenePathSplit = { "/", ".unity" };   //ReadOnly的数组，用来，切割舍弃掉/和.unity
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (EditorBuildSettings.scenes.Length == 0) return;

        //还没开始找，默认-1，那么开始找
        if(sceneIndex == -1)
        {
            GetSceneNameArray(property);
        }
        // 做点击事件，以及点击变更值
        int oldIndex = sceneIndex;
        sceneIndex = EditorGUI.Popup(position, label, sceneIndex, sceneNames);

        if (oldIndex != sceneIndex)
        {
            property.stringValue = sceneNames[sceneIndex].text;
        }
    }
    private void GetSceneNameArray(SerializedProperty property)
    {
        var scenes = EditorBuildSettings.scenes;
        //初始化数组
        sceneNames = new GUIContent[scenes.Length];

        for(int i = 0; i < sceneNames.Length; i++)
        {
            string path = scenes[i].path;
            string[] splitPath = path.Split(scenePathSplit, System.StringSplitOptions.RemoveEmptyEntries);

            string sceneName = "";
            if (splitPath.Length > 0)
            {
                sceneName = splitPath[splitPath.Length - 1];
            }
            else
            {
                sceneName = "(Deleted Scene)";
            }
            sceneNames[i] = new GUIContent(sceneName);
        }
        if (sceneNames.Length == 0)
        {
            //如果在EditorBuildSettings中没有找到场景，那么
            sceneNames = new[] { new GUIContent("请检查你的Build Settings的场景") };
        }
        // 注意！这里的！，要求property是非空，那么就选择当前已经有的场景序列
        if (!string.IsNullOrEmpty(property.stringValue))
        {
            bool nameFound = false;
            for(int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i].text == property.stringValue)
                {
                    sceneIndex = i;
                    nameFound = true;
                    break;
                }
            }
            if (nameFound == false)
            {
                //如果经过遍历，还是没有找到符合条件的name。那么取第一个场景0
                sceneIndex = 0;
            }
        }
        else
        {
            sceneIndex = 0;
        }
        property.stringValue = sceneNames[sceneIndex].text;
    }
}
