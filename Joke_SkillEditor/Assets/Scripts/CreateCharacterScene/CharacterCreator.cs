using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
/// <summary>
/// 角色创建者
/// </summary>
public class CharacterCreator : SingletonMono<CharacterCreator>
{
    [SerializeField] Player_View player_View;
    [SerializeField] Transform characterTransform;
    [SerializeField] Animator animator;

    // 不同职业的预览动画
    [SerializeField] RuntimeAnimatorController[] animatorControllers;

    // 不同职业的武器
    [SerializeField] GameObject[] warriorWeapons;
    [SerializeField] GameObject[] assassinWeapons;
    [SerializeField] GameObject[] archerWeapons;
    [SerializeField] GameObject[] tankeWeapons;
    // 当前的武器
    private GameObject[] currentWeapons;

    public void Init()
    {
        player_View.Init(DataManager.CustomCharacterData);
    }

    /// <summary>
    /// 设置职业
    /// </summary>
    public void SetProfession(ProfessionType professionType)
    {
        // 设置预览动画
        animator.runtimeAnimatorController = animatorControllers[(int)professionType];

        // 设置武器
        // 隐藏当前的武器
        if (currentWeapons!=null)
        {
            for (int i = 0; i < currentWeapons.Length; i++)
            {
                currentWeapons[i].SetActive(false);
            }
        }

        // 设置职业确定当前的武器数组
        switch (professionType)
        {
            case ProfessionType.Warrior:
                currentWeapons = warriorWeapons;
                break;
            case ProfessionType.Assassin:
                currentWeapons = assassinWeapons;
                break;
            case ProfessionType.Archer:
                currentWeapons = archerWeapons;
                break;
            case ProfessionType.Tanke:
                currentWeapons = tankeWeapons;
                break;
        }

        // 显示当前的武器
        for (int i = 0; i < currentWeapons.Length; i++)
        {
            currentWeapons[i].SetActive(true);
        }

    }

    /// <summary>
    /// 旋转角色
    /// </summary>
    public void RotateCharacter(Vector3 rot)
    {
        characterTransform.Rotate(rot);
    }

    /// <summary>
    /// 设置部位
    /// </summary>
    public void SetPart(CharacterPartConfigBase characterPartConfig, bool updateCharacterView)
    {
        player_View.SetPart(characterPartConfig,updateCharacterView);
    }

    public void SetSize(CharacterPartType characterPartType,float size)
    { 
        player_View.SetSize(characterPartType,size);
    }

    public void SetHieght(CharacterPartType characterPartType, float height)
    {
        player_View.SetHeight(characterPartType, height);
    }

    public void SetColor1(CharacterPartType characterPartType, Color color)
    {
        player_View.SetColor1(characterPartType, color);
    }
    public void SetColor2(CharacterPartType characterPartType, Color color)
    {
        player_View.SetColor2(characterPartType, color);
    }

    /// <summary>
    /// 获取当前的角色配置
    /// </summary>
    public CharacterPartConfigBase GetCurrentPartConfig(CharacterPartType characterPartType)
    {
        return player_View.GetCurrentPartConfig(characterPartType);
    }
}
