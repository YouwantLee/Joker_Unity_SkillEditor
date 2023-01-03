using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;

public class Player_SkillState : PlayerStateBase
{
    private SkillConfig skillConfig;
    private CharacterController characterController;

    public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
    {
        base.Init(owner, stateType, stateMachine);
        characterController = player.CharacterController;
    }

    public override void Enter()
    {
        skillConfig = ResManager.LoadAsset<SkillConfig>("SkillConfig");
        player.Skill_Player.PlaySkill(skillConfig, OnSkillEnd, onRootMotion);
    }

    private void onRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        characterController.Move(deltaPosition);
        player.ModelTransform.rotation *= deltaRotation;

    }

    private void OnSkillEnd()
    {
        player.ChangeState(PlayerState.Idle);
    }


}
