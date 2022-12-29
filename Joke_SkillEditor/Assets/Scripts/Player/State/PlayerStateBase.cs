using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JKFrame;

/// <summary>
/// 玩家状态的基类
/// </summary>
public abstract class PlayerStateBase:StateBase
{
    protected Player_Controller player;
    public override void Init(IStateMachineOwner owner, int stateType, StateMachine stateMachine)
    {
        base.Init(owner, stateType, stateMachine);
        player = (Player_Controller)owner;
    }
}
