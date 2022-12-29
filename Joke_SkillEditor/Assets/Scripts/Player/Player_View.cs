using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
/// <summary>
/// 玩家视图
/// </summary>
public class Player_View : MonoBehaviour
{
    [SerializeField] new Animation_Controller animation;
    public Animation_Controller Animation { get => animation; }

    [SerializeField] SkinnedMeshRenderer[] partSkinnedMeshRenderers;    // 部位的渲染器
    [SerializeField] Material[] partMaterials;                          // 部位的材质资源
    [SerializeField] Transform neckRootTransform;                       // 头部的根节点
    private CustomCharacterData customCharacterData;                    // 玩家自定义的角色数据-用于存档
    private Dictionary<int, CharacterPartConfigBase> characterPartDic = new Dictionary<int, CharacterPartConfigBase>(); // 角色部位字典
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(CustomCharacterData customCharacterData)
    {
        animation.Init();
        // 让每一个部位的材质都实例化一份自己的材质球，互不干扰
        partSkinnedMeshRenderers[0].material = Instantiate(partMaterials[0]);
        partSkinnedMeshRenderers[1].material = Instantiate(partMaterials[0]);
        partSkinnedMeshRenderers[2].material = Instantiate(partMaterials[2]);
        this.customCharacterData = customCharacterData;
    }

    /// <summary>
    /// 游戏中的初始化
    /// </summary>
    public void InitOnGame(CustomCharacterData customCharacterData)
    {
        Init(customCharacterData);
        // 基于数据设置当前部位
        CharacterPartConfigBase faceConfig = ConfigTool.LoadCharacterPartConfig(CharacterPartType.Face, customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Face].Index);
        CharacterPartConfigBase clothCofig = ConfigTool.LoadCharacterPartConfig(CharacterPartType.Cloth, customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Cloth].Index);
        CharacterPartConfigBase hairConfig = ConfigTool.LoadCharacterPartConfig(CharacterPartType.Hair, customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Hair].Index);
        CustomCharacterPartData facePartData = customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Face];
        
        SetPart(faceConfig,true);
        SetPart(clothCofig, true);
        SetPart(hairConfig, true);

        SetSize(CharacterPartType.Face, facePartData.Size);
        SetHeight(CharacterPartType.Face, facePartData.Height);
    }

    /// <summary>
    /// 获取当前的角色配置
    /// </summary>
    public CharacterPartConfigBase GetCurrentPartConfig(CharacterPartType characterPartType)
    {
        if (characterPartDic.TryGetValue((int)characterPartType, out CharacterPartConfigBase characterPartConfig))
        {
            return characterPartConfig;
        }
        return null;
    }

    /// <summary>
    /// 设置部位
    /// </summary>
    public void SetPart(CharacterPartConfigBase characterPartConfig, bool updateCharacterView = true)
    {
        if (characterPartDic.TryGetValue((int)characterPartConfig.CharacterPartType,out CharacterPartConfigBase currPartConfig))
        {
            // 释放旧配置的资源
            ResManager.Release<CharacterPartConfigBase>(currPartConfig);
            characterPartDic[(int)characterPartConfig.CharacterPartType] = characterPartConfig;
        }
        else
        {
            // 这个部位之前是空的，所以不存在资源释放问题
            characterPartDic.Add((int)characterPartConfig.CharacterPartType, characterPartConfig);
        }

        // 不更新实际的画面
        if (!updateCharacterView) return;

        switch (characterPartConfig.CharacterPartType)
        {
            case CharacterPartType.Face:
                partSkinnedMeshRenderers[0].sharedMesh = characterPartConfig.Mesh1;
                break;
            case CharacterPartType.Hair:
                HairConfig hairConfig = characterPartConfig as HairConfig;
                partSkinnedMeshRenderers[1].sharedMesh = hairConfig.Mesh1;
                SetColor1(CharacterPartType.Hair, customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Hair].Color1.ConverToUnityColor());
                break;
            case CharacterPartType.Cloth:
                ClothConfig clothConfig = characterPartConfig as ClothConfig;
                partSkinnedMeshRenderers[2].sharedMesh = clothConfig.Mesh1;
                SetColor1(CharacterPartType.Cloth, customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Cloth].Color1.ConverToUnityColor());
                SetColor2(CharacterPartType.Cloth, customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Cloth].Color2.ConverToUnityColor());
                break;
        }
    }

    /// <summary>
    /// 设置颜色1
    /// </summary>
    public void SetColor1(CharacterPartType characterPartType, Color color)
    {
        CharacterPartConfigBase partConfig = GetCurrentPartConfig(characterPartType);
        // 根据不同的部位类型，确定到具体要改材质球中的哪一个颜色
        switch (characterPartType)
        {
            case CharacterPartType.Hair:
                HairConfig hairConfig = partConfig as HairConfig;
                if (hairConfig.ColorIndex >= 0)
                {
                    partSkinnedMeshRenderers[1].sharedMaterial.SetColor("_Color0" + (hairConfig.ColorIndex + 1), color);
                }
                break;
            case CharacterPartType.Cloth:
                ClothConfig clothConfig = partConfig as ClothConfig;
                if (clothConfig.ColorIndex1 >= 0)
                {
                    partSkinnedMeshRenderers[2].sharedMaterial.SetColor("_Color0" + (clothConfig.ColorIndex1 + 1), color);
                }
                break;
        }
    }
    /// <summary>
    /// 设置颜色2
    /// </summary>
    public void SetColor2(CharacterPartType characterPartType, Color color)
    {
        CharacterPartConfigBase partConfig = GetCurrentPartConfig(characterPartType);
        // 根据不同的部位类型，确定到具体要改材质球中的哪一个颜色
        switch (characterPartType)
        {
            case CharacterPartType.Cloth:
                ClothConfig clothConfig = partConfig as ClothConfig;
                if (clothConfig.ColorIndex2>=0)
                {
                    partSkinnedMeshRenderers[2].sharedMaterial.SetColor("_Color0" + (clothConfig.ColorIndex2 + 1), color);
                }
                break;
        }
    }
    /// <summary>
    /// 设置某个部位的尺寸
    /// </summary>
    public void SetSize(CharacterPartType characterPartType, float size)
    {
        if (characterPartType == CharacterPartType.Face)
        {
            neckRootTransform.localScale = Vector3.one * size;
        }
    }

    /// <summary>
    /// 设置某个部位的高度
    /// </summary>
    public void SetHeight(CharacterPartType characterPartType, float height)
    {
        if (characterPartType == CharacterPartType.Face)
        {
            neckRootTransform.localPosition = new Vector3(-height, 0, 0);
        }
    }

    private void OnDestroy()
    {
        // 释放全部资源
        foreach (var item in characterPartDic)
        {
            ResManager.Release(item.Value);
        }
    }
}

