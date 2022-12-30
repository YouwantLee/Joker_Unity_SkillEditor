using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationTrack : SkillTrackBase
{
    public override string MenuAssetPath => "Assets/SkillEditor/Editor/Track/AnimationTrack/AnimationTrackMenu.uxml";
    public override string TrackAssetPath => "Assets/SkillEditor/Editor/Track/AnimationTrack/AnimationTrackContent.uxml";

    private Dictionary<int, AnimationTrackItem> trackItemDic = new Dictionary<int, AnimationTrackItem>();

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        track.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        track.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);

        RestView();
    }

    public override void RestView(float frameWidth)
    {
        base.RestView(frameWidth);
        //销毁当前已有
        foreach (var item in trackItemDic)
        {
            track.Remove(item.Value.root);
        }

        trackItemDic.Clear();
        if (SkillEditorWindows.Instance.SkillConfig == null) return;

        foreach (var item in SkillEditorWindows.Instance.SkillConfig.SkillAnimationData.FrameDataDic)
        {
            AnimationTrackItem trackItem = new AnimationTrackItem();
            trackItem.Init(this, track, item.Key, frameWidth, item.Value);
            trackItemDic.Add(item.Key, trackItem);
        }

        //根据数据绘制 TrackItem

    }

    #region  拖拽资源
    private void OnDragUpdatedEvent(DragUpdatedEvent evt)
    {
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        AnimationClip clip = objs[0] as AnimationClip;
        if (clip != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }

    private void OnDragExitedEvent(DragExitedEvent evt)
    {
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        AnimationClip clip = objs[0] as AnimationClip;
        if (clip != null)
        {
            //放置动画资源

            //当前选中的帧数位置 检测是否能放置动画
            int selectFrameIndex = SkillEditorWindows.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
            bool canPlace = true;
            int durationFrame = -1;//-1 代表可以用原本 AnimationClip 的持续时间
            int clipFrameCount = (int)(clip.length * clip.frameRate);
            int nextTrackItem = -1;
            int currentOffset = int.MaxValue;

            foreach (var item in SkillEditorWindows.Instance.SkillConfig.SkillAnimationData.FrameDataDic)
            {
                //不允许选中帧在 TrackItem 中间（动画事件的起点到他的终点之间）
                if (selectFrameIndex > item.Key && selectFrameIndex < item.Key + item.Value.DurationFrame)
                {
                    //不能放置
                    canPlace = false;
                    break;
                }

                //找到右侧的最近 TrackItem
                if (item.Key > selectFrameIndex)
                {
                    int tempOffset = item.Key - selectFrameIndex;
                    if (tempOffset < currentOffset)
                    {
                        currentOffset = tempOffset;
                        nextTrackItem = item.Key;
                    }
                }
            }

            //实际的放置
            if (canPlace)
            {
                // 右边有其他 TrackItem ，要考虑 Track 不能重叠的问题
                if (nextTrackItem != -1)
                {
                    int offset = clipFrameCount - currentOffset;
                    durationFrame = offset < 0 ? clipFrameCount : currentOffset; //计算这个空间能不能完整将动画片段放进去
                }
                else
                {
                    //右侧啥都没有
                    durationFrame = clipFrameCount;
                }

                //构建动画数据
                SkillAnimationEvent animationEvent = new SkillAnimationEvent()
                {
                    AnimationClip = clip,
                    DurationFrame = durationFrame,
                    TransitionTime = 0.25f
                };

                //保存新增的动画数据
                SkillEditorWindows.Instance.SkillConfig.SkillAnimationData.FrameDataDic.Add(selectFrameIndex, animationEvent);
                SkillEditorWindows.Instance.SaveConfig();

                //同步修改编辑器视图
            }


        }

    }

    #endregion


}
