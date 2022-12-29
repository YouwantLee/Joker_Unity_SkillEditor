using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;
/// <summary>
/// 项目配置
/// </summary>
[CreateAssetMenu(fileName = "ProjectConfig",menuName = "Config/ProjectConfig")]
public class ProjectConfig : ConfigBase
{
    #region 自定义角色窗口
    [BoxGroup("自定义角色窗口"),LabelText("自定角色的配置ID")]
    public Dictionary<CharacterPartType, List<int>> CustomCharacterPartConfigIDDic;
    #endregion
}
