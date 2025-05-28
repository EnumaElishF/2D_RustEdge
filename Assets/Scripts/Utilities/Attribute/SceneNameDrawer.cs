using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]
public class SceneNameDrawer : PropertyDrawer
{
    int sceneIndex = -1;
    GUIContent[] sceneNames;
    readonly string[] scenePathSplit = { "/", ".unity" };   //ReadOnly�����飬�������и�������/��.unity
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (EditorBuildSettings.scenes.Length == 0) return;

        //��û��ʼ�ң�Ĭ��-1����ô��ʼ��
        if(sceneIndex == -1)
        {
            GetSceneNameArray(property);
        }
        // ������¼����Լ�������ֵ
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
        //��ʼ������
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
            //�����EditorBuildSettings��û���ҵ���������ô
            sceneNames = new[] { new GUIContent("�������Build Settings�ĳ���") };
        }
        // ע�⣡����ģ���Ҫ��property�Ƿǿգ���ô��ѡ��ǰ�Ѿ��еĳ�������
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
                //�����������������û���ҵ�����������name����ôȡ��һ������0
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
