using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能播放器
/// </summary>
public class Skill_Player : MonoBehaviour
{
    private Animation_Controller animation_Controller;

    private bool isPlaying = false;     //当前是否处于播放状态
    public bool IsPlaying { get => isPlaying; }


    private SkillConfig skillConfig;    //当前播放的技能配置
    private int currentFrameIndex;      //当前是第几帧
    private float playTotalTime;        //当前播放的总时间
    private int frameRate;              //当前技能的帧率

    public void Init(Animation_Controller animation_Controller)
    {
        this.animation_Controller = animation_Controller;
    }

    private Action<Vector3, Quaternion> rootMotionAction;
    private Action skillEndAction;

    /// <summary>
    /// 播放技能
    /// </summary>
    /// <param name="skillConfig"> 技能配置 </param>
    public void PlaySkill(SkillConfig skillConfig, Action skillEndAction, Action<Vector3, Quaternion> rootMotionAction = null)
    {
        this.skillConfig = skillConfig;
        this.skillEndAction = skillEndAction;
        this.rootMotionAction = rootMotionAction;

        currentFrameIndex = -1;
        frameRate = skillConfig.FrameRate;
        playTotalTime = 0;
        isPlaying = true;

        TickSkill();
    }

    private void Update()
    {
        if (isPlaying)
        {
            playTotalTime += Time.deltaTime;
            //根据总时间判断当前是第几帧
            int targetFrameIndex = (int)(playTotalTime * frameRate);
            //防止一帧延迟过大，追帧
            while (currentFrameIndex < targetFrameIndex)
            {
                //驱动一次技能
                TickSkill();
            }

            //如果达到最后一帧，技能结束
            if (targetFrameIndex >= skillConfig.FrameCount)
            {
                isPlaying = false;
                skillConfig = null;
                if (rootMotionAction != null) animation_Controller.ClearRootMotionAction();
                rootMotionAction = null;
                skillEndAction?.Invoke();
            }
        }
    }

    private void TickSkill()
    {
        currentFrameIndex += 1;
        //驱动动画
        if (animation_Controller != null && skillConfig.SkillAnimationData.FrameDataDic.TryGetValue(currentFrameIndex, out SkillAnimationEvent skillAnimationEvent))
        {
            animation_Controller.PlaySingleAniamtion(skillAnimationEvent.AnimationClip, 1, true, skillAnimationEvent.TransitionTime);

            if (skillAnimationEvent.ApplyRootMotion)
            {
                animation_Controller.SetRootMotionAction(rootMotionAction);
            }
            else
            {
                animation_Controller.ClearRootMotionAction();

            }

        }

    }

}
