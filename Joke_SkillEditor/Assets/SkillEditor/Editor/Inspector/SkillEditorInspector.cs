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
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                root.RemoveAt(i);
            }
        }
    }


    private void DrawAnimationTrackItem(AnimationTrackItem animationTrackItem)
    {
        //动画资源
        ObjectField animationClipAssetField = new ObjectField("动画资源");
        animationClipAssetField.objectType = typeof(AnimationClip);
        animationClipAssetField.value = animationTrackItem.AnimationEvent.AnimationClip;
        root.Add(animationClipAssetField);

        //轨道长度
        IntegerField duration = new IntegerField("轨道长度");
        duration.value = animationTrackItem.AnimationEvent.DurationFrame;
        root.Add(duration);

        //过渡时间
        FloatField transitionTime = new FloatField("过渡时间");
        transitionTime.value = animationTrackItem.AnimationEvent.TransitionTime;
        root.Add(transitionTime);

        //动画相关的信息
        int clipFrameCount = (int)(animationTrackItem.AnimationEvent.AnimationClip.length * animationTrackItem.AnimationEvent.AnimationClip.frameRate);
        Label clipFrame = new Label("动画资源长度：" + clipFrameCount);
        root.Add(clipFrame);

        Label isLoopLable = new Label("循环动画：" + animationTrackItem.AnimationEvent.AnimationClip.isLooping);
        root.Add(isLoopLable);

        //删除
        Button deleteButton = new Button();
        deleteButton.text = "删除";
        deleteButton.style.backgroundColor = new Color(1, 0, 0, 0.5f);
        root.Add(deleteButton);



    }

}
