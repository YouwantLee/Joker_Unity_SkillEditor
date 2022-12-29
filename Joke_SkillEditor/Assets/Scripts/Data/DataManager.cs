using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using JKFrame;
/// <summary>
/// 数据管理器
/// </summary>
public static class DataManager
{
    static DataManager()
    {
        LoadSaveData();
    }

    /// <summary>
    /// 是否有存档
    /// </summary>
    public static bool HaveArchive { get; private set; }

    private static void LoadSaveData()
    {
        SaveItem saveItem = SaveManager.GetSaveItem(0);
        HaveArchive = saveItem != null;
    }

    /// <summary>
    /// 创建新存档
    /// </summary>
    public static void CreateArchive()
    {
        if (HaveArchive)
        {
            // 删除全部存档
            SaveManager.DeleteAllSaveItem();
        }

        // 创建一个存档
        SaveManager.CreateSaveItem();

        // 初始化角色外观数据
        InitCustomCharacterData();
        SaveCustomCharacterData();
    }

    /// <summary>
    /// 加载当前存档
    /// </summary>
    public static void LoadCurrentArchive()
    {
        CustomCharacterData = SaveManager.LoadObject<CustomCharacterData>();
    }

    #region 玩家数据
    public static CustomCharacterData CustomCharacterData;
    public static void InitCustomCharacterData()
    {
        CustomCharacterData = new CustomCharacterData();
        CustomCharacterData.CustomPartDataDic = new Serialization_Dic<int, CustomCharacterPartData>();
        CustomCharacterData.CustomPartDataDic.Dictionary.Add((int)CharacterPartType.Face, new CustomCharacterPartData()
        {
            Index = 1,
            Size = 1,
            Height = 0
        });
        CustomCharacterData.CustomPartDataDic.Dictionary.Add((int)CharacterPartType.Hair, new CustomCharacterPartData()
        {
            Index = 1,
            Color1 = Color.white.ConverToSerializationColor()
        }); ;
        CustomCharacterData.CustomPartDataDic.Dictionary.Add((int)CharacterPartType.Cloth, new CustomCharacterPartData()
        {
            Index = 1,
            Color1 = Color.white.ConverToSerializationColor(),
            Color2 = Color.black.ConverToSerializationColor()
        });
    }
    public static void SaveCustomCharacterData()
    {
        SaveManager.SaveObject(CustomCharacterData);
    }
    #endregion

}
