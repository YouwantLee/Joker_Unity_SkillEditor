using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationTrackItem : TrackItemBase
{
    private const string trackItemAssetPath = "Assets/SkillEditor/Editor/Track/AnimationTrack/AnimationTrackItem.uxml";
    private AnimationTrack animationTrack;
    private int frameIndex;
    private float frameUnitWidth;
    private SkillAnimationEvent animationEvent;

    public Label root { get; private set; }
    private VisualElement mainDragArea;
    private VisualElement animationOverLine;

    private static Color normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
    private static Color selectColor = new Color(0.388f, 0.850f, 0.905f, 1f);

    private bool mouseDrag = false;

    public void Init(AnimationTrack animationTrack, VisualElement parent, int startFrameIndex, float frameUnitWidth, SkillAnimationEvent animationEvent)
    {
        this.animationTrack = animationTrack;
        this.frameIndex = startFrameIndex;
        this.frameUnitWidth = frameUnitWidth;
        this.animationEvent = animationEvent;

        root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();//不要容器，直接持有目标物体
        mainDragArea = root.Q<VisualElement>("Main");
        animationOverLine = root.Q<VisualElement>("OverLine");
        parent.Add(root);

        //绑定事件
        mainDragArea.RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
        mainDragArea.RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
        mainDragArea.RegisterCallback<MouseOutEvent>(OnMouseOutEvent);
        mainDragArea.RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);

        RestView(frameUnitWidth);
    }

    public void RestView(float frameUnitWidth)
    {
        this.frameUnitWidth = frameUnitWidth;
        root.text = animationEvent.AnimationClip.name;

        //位置计算
        Vector3 mainPos = root.transform.position;
        mainPos.x = frameIndex * frameUnitWidth;
        root.transform.position = mainPos;
        root.style.width = animationEvent.DurationFrame * frameUnitWidth;

        //计算动画结束线的位置
        int animationClipFrameCount = (int)(animationEvent.AnimationClip.length * animationEvent.AnimationClip.frameRate);
        if (animationClipFrameCount > animationEvent.DurationFrame)
        {
            animationOverLine.style.display = DisplayStyle.None;
        }
        else
        {
            animationOverLine.style.display = DisplayStyle.Flex;
            Vector3 overLinePos = animationOverLine.transform.position;
            //overLinePos.x = animationClipFrameCount * frameUnitWidth - animationOverLine.style.width.value.value / 2;
            overLinePos.x = animationClipFrameCount * frameUnitWidth - 1; //线条宽度为2，取一半
            animationOverLine.transform.position = overLinePos;
        }

    }


    private void OnMouseDownEvent(MouseDownEvent evt)
    {
        root.style.backgroundColor = selectColor;
        mouseDrag = true;
    }

    private void OnMouseUpEvent(MouseUpEvent evt)
    {
        mouseDrag = false;
    }

    private void OnMouseOutEvent(MouseOutEvent evt)
    {
        root.style.backgroundColor = normalColor;
        mouseDrag = false;
    }

    private void OnMouseMoveEvent(MouseMoveEvent evt)
    {
        if (mouseDrag)
        {
            Debug.Log(evt.mousePosition.x);
        }
    }


}
