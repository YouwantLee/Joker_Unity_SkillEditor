using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class SkillTrackBase
{
    protected VisualElement menuParent;
    protected VisualElement trackParent;
    protected VisualElement menu;
    protected VisualElement track;

    public abstract string MenuAssetPath { get; }
    public abstract string TrackAssetPath { get; }

    public virtual void Init(VisualElement menuParent, VisualElement trackParent)
    {
        this.menuParent = menuParent;
        this.trackParent = trackParent;

        menu = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
        track = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
        menuParent.Add(menu);
        trackParent.Add(track);
    }



}
