using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class SkillEditorWindows : EditorWindow
{ 
    [MenuItem("SkillEditor/SkillEditorWindows")]
    public static void ShowExample()
    {
        SkillEditorWindows wnd = GetWindow<SkillEditorWindows>();
        wnd.titleContent = new GUIContent("¼¼ÄÜ±à¼­Æ÷");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SkillEditor/Editor/EditorWindows/SkillEditorWindows.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);
    }
}