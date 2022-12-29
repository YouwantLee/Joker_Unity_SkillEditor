using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
public class CreateCharacterSceneManager : LogicManagerBase<CreateCharacterSceneManager>
{
    private void Start()
    {
        // 初始化角色创建者
        CharacterCreator.Instance.Init();
        // 显示创建角色的主窗口
        UIManager.Instance.Show<UI_CreateCharacterWindow>();
    }
}
