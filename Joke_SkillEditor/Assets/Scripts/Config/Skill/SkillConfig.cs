using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;
using System;
using Sirenix.Serialization;

[CreateAssetMenu(menuName = "Config/SkillConfig", fileName = "SkillConfig")]
public class SkillConfig : ConfigBase
{
    [LabelText("技能名称")] public string SkillName;
    [LabelText("帧数上限")] public int FrameCount = 100;
    [LabelText("帧率")] public int FrameRate = 30;

    [NonSerialized, OdinSerialize]
    public SkillAnimationData SkillAnimationData = new SkillAnimationData();

#if UNITY_EDITOR
    private static Action onSkillConfigValidate;

    public static void SetValidateAction(Action action)
    {
        onSkillConfigValidate = action;
    }

    private void OnValidate()
    {
        onSkillConfigValidate?.Invoke();
    }
#endif

}

/// <summary>
/// 技能动画数据
/// </summary>
[Serializable]
public class SkillAnimationData
{
    /// <summary>
    /// 动画帧事件
    /// key:帧数
    /// value：事件数据
    /// </summary>
    [NonSerialized, OdinSerialize]//不通过Unity的序列化，用Odin 的序列化
    [DictionaryDrawerSettings(KeyLabel = "帧数", ValueLabel = "动画数据")]
    public Dictionary<int, SkillAnimationEvent> FrameDataDic = new Dictionary<int, SkillAnimationEvent>();
}

/// <summary>
/// 帧事件基类
/// </summary>
[Serializable]
public abstract class SkillFrameEventBase
{

}

public class SkillAnimationEvent : SkillFrameEventBase
{
    public AnimationClip AnimationClip;
    public float TransitionTime = 0.25f;
    public bool ApplyRootMotion;

#if UNITY_EDITOR
    public int DurationFrame;

#endif

}