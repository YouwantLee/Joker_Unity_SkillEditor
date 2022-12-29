using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using Sirenix.OdinInspector;

/// <summary>
/// 角色的部位类型
/// </summary>
public enum CharacterPartType
{ 
    [LabelText("脸部")] Face,
    [LabelText("头发")] Hair,
    [LabelText("腰带")] Belt,
    [LabelText("上衣")] Cloth,
    [LabelText("帽子")] Hat,
    [LabelText("手套")] Glove,
    [LabelText("鞋子")] Shoe,
    [LabelText("肩部")] ShoulderPad,
}

/// <summary>
/// 部位配置
/// </summary>
public abstract class CharacterPartConfigBase:ConfigBase
{
    [LabelText("名称")] public string Name;
    [LabelText("支持的职业")] public List<ProfessionType> ProfessionTypes;
    [LabelText("部位")] public CharacterPartType CharacterPartType;
    [LabelText("主网格")] public Mesh Mesh1;
}
