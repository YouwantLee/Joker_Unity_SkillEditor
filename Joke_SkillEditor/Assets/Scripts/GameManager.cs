using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
public class GameManager : SingletonMono<GameManager>
{

    /// <summary>
    /// 创建新存档，并且进入游戏
    /// </summary>
    public void CreateNewArchiveAndEnterGame()
    {
        // 初始化存档
        DataManager.CreateArchive();
        // 进入自定义角色场景
        SceneManager.LoadScene("CreateCharacter");
    }

    /// <summary>
    /// 使用就存档，进入游戏
    /// </summary>
    public void UseCurrentArchiveAndEnterGame()
    { 
        // 加载当前存档
        DataManager.LoadCurrentArchive();
        // 进入游戏场景
        SceneManager.LoadScene("Game");
    }
}
