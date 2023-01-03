using UnityEngine;

/// <summary>
/// 玩家待机状态
/// </summary>
public class Player_IdleState : PlayerStateBase
{
    public override void Enter()
    {
        // 播放待机动作
        player.PlayAnimation("Idle");
    }

    public override void Update()
    {
        // 检测玩家的输入
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            // 切换状态
            player.ChangeState(PlayerState.Move);
        }

        //测试进入技能
        if (Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.Skill);
        }

    }
}
