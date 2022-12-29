using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConfig",menuName = "Config/CharacterConfig")]
public class CharacterConfig:ConfigBase
{
    [LabelText("走路速度")]public float WalkSpeed;
    [LabelText("奔跑速度")] public float RunSpeed;
    [LabelText("走路到奔跑过渡速度")] public float Walk2RunTransitionSpeed;
    [LabelText("脚步声资源")] public AudioClip[] FootStepAudioClips;
    [LabelText("旋转速度")] public float RotateSpeed;
    [LabelText("为移动应用RootMotion")] public bool ApplyRootMotionForMove;
    [LabelText("标准动画表")] public Dictionary<string, AnimationClip> StandAnimationDic;

    public AnimationClip GetAnimationByName(string animationName)
    {
        return StandAnimationDic[animationName];
    }
}
