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
        //���ٵ�ǰ����
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

        //�������ݻ��� TrackItem

    }

    #region  ��ק��Դ
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
            //���ö�����Դ

            //��ǰѡ�е�֡��λ�� ����Ƿ��ܷ��ö���
            int selectFrameIndex = SkillEditorWindows.Instance.GetFrameIndexByPos(evt.localMousePosition.x);
            bool canPlace = true;
            int durationFrame = -1;//-1 ����������ԭ�� AnimationClip �ĳ���ʱ��
            int clipFrameCount = (int)(clip.length * clip.frameRate);
            int nextTrackItem = -1;
            int currentOffset = int.MaxValue;

            foreach (var item in animationData.FrameDataDic)
            {
                //������ѡ��֡�� TrackItem �м䣨�����¼�����㵽�����յ�֮�䣩
                if (selectFrameIndex > item.Key && selectFrameIndex < item.Key + item.Value.DurationFrame)
                {
                    //���ܷ���
                    canPlace = false;
                    break;
                }

                //�ҵ��Ҳ����� TrackItem
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

            //ʵ�ʵķ���
            if (canPlace)
            {
                // �ұ������� TrackItem ��Ҫ���� Track �����ص�������
                if (nextTrackItem != -1)
                {
                    int offset = clipFrameCount - currentOffset;
                    durationFrame = offset < 0 ? clipFrameCount : currentOffset; //��������ռ��ܲ�������������Ƭ�ηŽ�ȥ
                }
                else
                {
                    //�Ҳ�ɶ��û��
                    durationFrame = clipFrameCount;
                }

                //������������
                SkillAnimationEvent animationEvent = new SkillAnimationEvent()
                {
                    AnimationClip = clip,
                    DurationFrame = durationFrame,
                    TransitionTime = 0.25f
                };

                //���������Ķ�������
                animationData.FrameDataDic.Add(selectFrameIndex, animationEvent);
                SkillEditorWindows.Instance.SaveConfig();

                //ͬ���޸ı༭����ͼ
                ResetView();
            }
        }
    }

    #endregion

    public bool CheckFrameIndexOnDrag(int targetindex)
    {
        foreach (var item in animationData.FrameDataDic)
        {
            //������ targetindex ��ĳ�� TrackItem �м�
            if (targetindex > item.Key && targetindex < item.Key + item.Value.DurationFrame)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// �� oldIndex �����ݱ�Ϊ newIndex
    /// </summary>
    public void SetFrameIndex(int oldIndex, int newIndex)
    {
        if (animationData.FrameDataDic.Remove(oldIndex, out SkillAnimationEvent animationEvent))
        {
            animationData.FrameDataDic.Add(newIndex, animationEvent);
            SkillEditorWindows.Instance.SaveConfig();
        }
    }

}