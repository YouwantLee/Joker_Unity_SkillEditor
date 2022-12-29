using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 头发配置
/// </summary>
[CreateAssetMenu(fileName = "HairConfig_", menuName = "Config/HairConfig")]
public class HairConfig : CharacterPartConfigBase
{
    /// <summary>
    /// 颜色索引,-1：意味着无效
    /// </summary>
    [LabelText("颜色Index")] public int ColorIndex;
    [LabelText("半头网格")] public Mesh Mesh2;
}
