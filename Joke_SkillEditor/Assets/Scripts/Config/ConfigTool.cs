using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
public static class ConfigTool
{
    public static string ProjectConfigName = "Project";
    /// <summary>
    /// 获取角色部位配置
    /// </summary>
    public static CharacterPartConfigBase LoadCharacterPartConfig(CharacterPartType characterPartType, int index)
    {
        string configAssetName = string.Empty;
        switch (characterPartType)
        {
            case CharacterPartType.Face:
                configAssetName = "FaceConfig_";
                break;
            case CharacterPartType.Hair:
                configAssetName = "HairConfig_";
                break;
            case CharacterPartType.Cloth:
                configAssetName = "ClothConfig_";
                break;
        }
        configAssetName = configAssetName + index.ToString();
        return ResManager.LoadAsset<CharacterPartConfigBase>(configAssetName);
    }
}
