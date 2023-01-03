using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillAnimationTrackItemStyle : SkillTrackItemStyleBase
{
    private const string trackItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/AnimationTrack/AnimationTrackItem.uxml";
    private Label titleLabel;
    public VisualElement mainDragArea { get; private set; }
    public VisualElement animationOverLine { get; private set; }

    public void Init(SkillTrackStyleBase TrackStyle, int startFrameIndex, float frameUnitWidth)
    {
        root = titleLabel = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackItemAssetPath).Instantiate().Query<Label>();//不要容器，直接持有目标物体
        mainDragArea = root.Q<VisualElement>("Main");
        animationOverLine = root.Q<VisualElement>("OverLine");
        TrackStyle.AddItem(root);
    }

    public void SetTitle(string title)
    {
        titleLabel.text = title;
    }



}
