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
    private SkillAnimationData animationData { get => SkillEditorWindows.Instance.SkillConfig.SkillAnimationData; }

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        track.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        track.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);

        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        //销毁当前已有
        foreach (var item in trackItemDic)
        {
            track.Remove(item.Value.root);
        }

        trackItemDic.Clear();
        if (SkillEditorWindows.Instance.SkillConfig == null) return;

        foreach (var item in animationData.FrameDataDic)
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

            foreach (var item in animationData.FrameDataDic)
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
                animationData.FrameDataDic.Add(selectFrameIndex, animationEvent);
                SkillEditorWindows.Instance.SaveConfig();

                //同步修改编辑器视图
                ResetView();
            }
        }
    }

    #endregion

    public bool CheckFrameIndexOnDrag(int targetindex, int selfIndex, bool isLeft)
    {
        foreach (var item in animationData.FrameDataDic)
        {
            //拖拽时，规避自身
            if (item.Key == selfIndex) continue;

            //向左移动&&原先在右边&&目标没有重叠
            if (isLeft && selfIndex > item.Key && targetindex < item.Key + item.Value.DurationFrame)
            {
                return false;
            }
            //向右移动&&原先在左边&&目标没有重叠
            else if (!isLeft && selfIndex < item.Key && targetindex > item.Key)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 将 oldIndex 的数据变为 newIndex
    /// </summary>
    public void SetFrameIndex(int oldIndex, int newIndex)
    {
        if (animationData.FrameDataDic.Remove(oldIndex, out SkillAnimationEvent animationEvent))
        {
            animationData.FrameDataDic.Add(newIndex, animationEvent);
            SkillEditorWindows.Instance.SaveConfig();
        }
    }

    public override void DeleteTrackItem(int frameIndex)
    {
        animationData.FrameDataDic.Remove(frameIndex);
        SkillEditorWindows.Instance.SaveConfig();
        ResetView();
    }


}
