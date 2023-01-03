using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationTrack : SkillTrackBase
{
    private SkillSingleLineTrackStyle trackStyle;

    private Dictionary<int, AnimationTrackItem> trackItemDic = new Dictionary<int, AnimationTrackItem>();
    public SkillAnimationData AnimationData { get => SkillEditorWindows.Instance.SkillConfig.SkillAnimationData; }

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillSingleLineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "动画配置");
        trackStyle.contentRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        trackStyle.contentRoot.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);

        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);
        //销毁当前已有
        foreach (var item in trackItemDic)
        {
            trackStyle.DeleteItem(item.Value.itemStyle.root);
        }

        trackItemDic.Clear();
        if (SkillEditorWindows.Instance.SkillConfig == null) return;

        //根据数据绘制 TrackItem
        foreach (var item in AnimationData.FrameDataDic)
        {
            CreateItem(item.Key, item.Value);
        }
    }

    private void CreateItem(int frameIndex, SkillAnimationEvent skillAnimationEvent)
    {
        AnimationTrackItem trackItem = new AnimationTrackItem();
        trackItem.Init(this, trackStyle, frameIndex, frameWidth, skillAnimationEvent);
        trackItemDic.Add(frameIndex, trackItem);
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

            foreach (var item in AnimationData.FrameDataDic)
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
                AnimationData.FrameDataDic.Add(selectFrameIndex, animationEvent);
                SkillEditorWindows.Instance.SaveConfig();

                //绘制一个Item
                CreateItem(selectFrameIndex, animationEvent);
            }
        }
    }

    #endregion

    public bool CheckFrameIndexOnDrag(int targetindex, int selfIndex, bool isLeft)
    {
        foreach (var item in AnimationData.FrameDataDic)
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
        if (AnimationData.FrameDataDic.Remove(oldIndex, out SkillAnimationEvent animationEvent))
        {
            AnimationData.FrameDataDic.Add(newIndex, animationEvent);
            trackItemDic.Remove(oldIndex, out AnimationTrackItem animationTrackItem);
            trackItemDic.Add(newIndex, animationTrackItem);

            SkillEditorWindows.Instance.SaveConfig();
        }
    }

    public override void DeleteTrackItem(int frameIndex)
    {
        //移除数据
        AnimationData.FrameDataDic.Remove(frameIndex);
        if (trackItemDic.Remove(frameIndex, out AnimationTrackItem item))
        {
            //移除视图
            trackStyle.DeleteItem(item.itemStyle.root);
        }
        SkillEditorWindows.Instance.SaveConfig();
    }

    public override void OnConfigChanged()
    {
        foreach (var item in trackItemDic.Values)
        {
            item.OnConfigChanged();
        }
    }

    public override void TickView(int frameIndex)
    {
        base.TickView(frameIndex);

        GameObject previewGameObject = SkillEditorWindows.Instance.PreviewCharacterObj;
        Animator animator = previewGameObject.GetComponent<Animator>();

        //根据帧找到目前是哪个动画
        Dictionary<int, SkillAnimationEvent> frameDateDic = AnimationData.FrameDataDic;

        #region 关于根运动计算
        SortedDictionary<int, SkillAnimationEvent> frameDataSortedDic = new SortedDictionary<int, SkillAnimationEvent>(frameDateDic);
        int[] keys = frameDataSortedDic.Keys.ToArray();
        Vector3 rootMotionTotalPos = Vector3.zero;

        //从第0帧开始累加位移坐标
        for (int i = 0; i < keys.Length; i++)
        {
            int key = keys[i]; //当前动画的起始帧数
            SkillAnimationEvent animationEvent = frameDataSortedDic[key];

            //只考虑根运动配置的动画
            if (animationEvent.ApplyRootMotion == false) continue;

            //找到后一个动画的帧起始位置
            int nextKeyFrame = i + 1 < keys.Length ? keys[i + 1] : SkillEditorWindows.Instance.SkillConfig.FrameCount;//最后一个动画

            //标记是最后一次采样
            bool isBreak = false;
            //下一帧大于当前选中帧（帧数累加完成，可以停止累加坐标的标志）
            if (nextKeyFrame > frameIndex)
            {
                nextKeyFrame = frameIndex;
                isBreak = true;
            }

            //持续帧数=下一个动画的帧数  -  这个动画的开始时间
            int durationFrameCount = nextKeyFrame - key;
            if (durationFrameCount > 0)
            {
                //动画资源的总帧数
                float clipFrameCount = animationEvent.AnimationClip.length * SkillEditorWindows.Instance.SkillConfig.FrameRate;
                //计算总的播放进度
                float totalProgress = durationFrameCount / clipFrameCount;
                //播放次数
                int playTimes = 0;
                //最终不完整的一次播放的进度
                float lastProgress = 0;
                //只有循环动画才需要采样多次
                if (animationEvent.AnimationClip.isLooping)
                {
                    playTimes = (int)totalProgress;
                    lastProgress = totalProgress - (int)totalProgress;
                }
                else
                {
                    // 不循环的动画，如果播放进度超过1，约束为1
                    if (totalProgress >= 1)
                    {
                        playTimes = 1;
                        lastProgress = 0;
                    }
                    else if (totalProgress < 1)
                    {
                        lastProgress = totalProgress; // 因为总进度小于1，所以本身就是最后一次播放
                        playTimes = 0;
                    }
                }

                //采样计算
                animator.applyRootMotion = true;
                if (playTimes >= 1)
                {
                    //采样一次动画的完整进度
                    animationEvent.AnimationClip.SampleAnimation(previewGameObject, animationEvent.AnimationClip.length);
                    Vector3 samplePos = previewGameObject.transform.position;
                    rootMotionTotalPos += samplePos * playTimes;
                }

                if (lastProgress > 0)
                {
                    //采样一次动画的不完整进度
                    animationEvent.AnimationClip.SampleAnimation(previewGameObject, lastProgress * animationEvent.AnimationClip.length);
                    Vector3 samplePos = previewGameObject.transform.position;
                    rootMotionTotalPos += samplePos;
                }
            }

            if (isBreak) break;
        }
        #endregion

        #region 关于当前帧的姿态
        //找到距离这一帧左边最近的一个动画，也就是当前要播放的动画
        int currentOffset = int.MaxValue;  //最近的索引距离当前选中帧的差距
        int animtionEventIndex = -1;
        foreach (var item in frameDateDic)
        {
            int tempOffset = frameIndex - item.Key;
            if (tempOffset > 0 && tempOffset < currentOffset)
            {
                currentOffset = tempOffset;
                animtionEventIndex = item.Key;
            }
        }

        if (animtionEventIndex != -1)
        {
            SkillAnimationEvent animationEvent = frameDateDic[animtionEventIndex];
            //动画资源总帧数
            float clipFrameCount = animationEvent.AnimationClip.length * animationEvent.AnimationClip.frameRate;
            //计算当前的播放进度
            float progress = currentOffset / clipFrameCount;
            //循环动画的处理
            if (progress > 1 && animationEvent.AnimationClip.isLooping)
            {
                progress -= (int)progress;//只保留小数点部分
            }

            //（此处会修改角色位置）
            animator.applyRootMotion = animationEvent.ApplyRootMotion;
            animationEvent.AnimationClip.SampleAnimation(previewGameObject, progress * animationEvent.AnimationClip.length);
        }
        #endregion

        //将角色拉回实际位置
        previewGameObject.transform.position = rootMotionTotalPos;
    }

    public override void Destory()
    {
        trackStyle.Destory();
    }

}
