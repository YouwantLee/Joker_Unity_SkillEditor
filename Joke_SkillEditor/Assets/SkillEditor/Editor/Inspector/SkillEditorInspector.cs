using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

[CustomEditor(typeof(SkillEditorWindows))]
public class SkillEditorInspector : Editor
{
    public static SkillEditorInspector Instance;
    private static TrackItemBase currentTrackItem;

    private VisualElement root;


    public static void SetTrackItem(TrackItemBase trackItem)
    {
        currentTrackItem = trackItem;
        if (Instance != null)
        {
            Instance.Show();  //避免已经打开Inspector，导致的面板刷新不及时
        }
    }

    public override VisualElement CreateInspectorGUI()
    {
        Instance = this;
        root = new VisualElement();
        root.Add(new Label("AAAA"));

        Show();
        return root;
    }


    private void Show()
    {
        Clean();
        if (currentTrackItem == null) return;

        //目前只有动画这一种情况
        if (currentTrackItem.GetType() == typeof(AnimationTrackItem))
        {
            DrawAnimationTrackItem((AnimationTrackItem)currentTrackItem);
        }

    }

    private void Clean()
    {
        if (root != null)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                root.RemoveAt(i);
            }
        }
    }


    private void DrawAnimationTrackItem(AnimationTrackItem animationTrackItem)
    {
        ObjectField animationClipAssetField = new ObjectField("动画资源");
        animationClipAssetField.objectType = typeof(AnimationClip);
        animationClipAssetField.value = animationTrackItem.AnimationEvent.AnimationClip;
        root.Add(animationClipAssetField);

    }

}
