using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class SkillTrackBase
{
    protected float frameWidth;
    protected VisualElement menuParent;
    protected VisualElement trackParent;
    protected VisualElement menu;
    protected VisualElement track;

    public abstract string MenuAssetPath { get; }
    public abstract string TrackAssetPath { get; }

    public virtual void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        this.menuParent = menuParent;
        this.trackParent = trackParent;
        this.frameWidth = frameWidth;

        menu = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(MenuAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
        track = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TrackAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
        menuParent.Add(menu);
        trackParent.Add(track);
    }

    /// <summary>
    /// 在当前宽高进行刷新（内部变化）
    /// </summary>
    public virtual void ResetView()
    {
        ResetView(frameWidth);
    }

    /// <summary>
    /// 宽高有变化的刷新（滚轮滑动）
    /// </summary>
    /// <param name="frameWidth"></param>
    public virtual void ResetView(float frameWidth)
    {
        this.frameWidth = frameWidth;
    }

    public virtual void DeleteTrackItem(int frameIndex)
    {

    }

}
