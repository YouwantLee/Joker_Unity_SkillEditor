using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using UnityEngine.Animations;
using System;

public class Player_Controller : SingletonMono<Player_Controller>, IStateMachineOwner
{
    [SerializeField] private Player_View view;
    [SerializeField] private Skill_Player skill_Player;
    public Skill_Player Skill_Player { get => skill_Player; }


    [SerializeField] private CharacterController characterController;
    public CharacterController CharacterController { get => characterController; }

    private StateMachine stateMachine;
    private PlayerState playerState; // 玩家的当前状态
    private CharacterConfig characterConfig;
    public CharacterConfig CharacterConfig { get => characterConfig; }
    public Animation_Controller Animation_Controller { get => view.Animation; }
    public Transform ModelTransform { get => view.transform; }
    public float WalkSpeed { get => characterConfig.WalkSpeed; }
    public float RunSpeed { get => characterConfig.RunSpeed; }
    public float RotateSpeed { get => characterConfig.RotateSpeed; }

    public void Init()
    {
        // TODO:之后根据不同职业，获取不同的角色配置
        characterConfig = ResManager.LoadAsset<CharacterConfig>("WarriorConfig");
        view.InitOnGame(DataManager.CustomCharacterData);
        skill_Player.Init(view.Animation);
        // 初始化状态机
        stateMachine = PoolManager.Instance.GetObject<StateMachine>();
        stateMachine.Init(this);
        // 默认状态为待机
        ChangeState(PlayerState.Idle);
    }

    /// <summary>
    /// 修改状态
    /// </summary>
    public void ChangeState(PlayerState playerState)
    {
        this.playerState = playerState;
        switch (playerState)
        {
            case PlayerState.Idle:
                stateMachine.ChangeState<Player_IdleState>((int)playerState);
                break;
            case PlayerState.Move:
                stateMachine.ChangeState<Player_MoveState>((int)playerState);
                break;
            case PlayerState.Skill:
                stateMachine.ChangeState<Player_SkillState>((int)playerState);
                break;
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation(string animationClipName, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f)
    {
        if (rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        view.Animation.PlaySingleAniamtion(characterConfig.GetAnimationByName(animationClipName), speed, refreshAnimation, transitionFixedTime);
    }

    /// <summary>
    /// 播放混合动画
    /// </summary>
    public void PlayBlendAnimation(string clip1Name, string clip2Name, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, float transitionFixedTime = 0.25f)
    {
        if (rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        AnimationClip clip1 = characterConfig.GetAnimationByName(clip1Name);
        AnimationClip clip2 = characterConfig.GetAnimationByName(clip2Name);
        view.Animation.PlayBlendAnimation(clip1, clip2, speed, transitionFixedTime);
    }



}
