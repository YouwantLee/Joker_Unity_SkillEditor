using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色创建窗口_外观菜单的选项
/// </summary>
public class UI_FacadeMenus_Tab : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image iconImg;
    [SerializeField] Image tabFocus;
    private UI_CreateCharacterWindow window;

    [SerializeField] AudioClip clickAudioClip;

    /// <summary>
    ///  当前菜单选项 代表的外观类型
    /// </summary>
    public CharacterPartType CharacterPartType { get; private set; }

    private static Color[] colors;
    static UI_FacadeMenus_Tab()
    {
        colors = new Color[2];
        colors[0] = Color.white;
        colors[1] = new Color(0.964f, 0.882f, 0.611f);
    }
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(UI_CreateCharacterWindow window,CharacterPartType characterPartType)
    {
        this.window = window;
        this.CharacterPartType = characterPartType;
        button.onClick.AddListener(ButtonClick);
        UnSelect();
    }

    private void ButtonClick()
    {
        // 告诉窗口，当前按钮代表的职业被玩家选择了
        AudioManager.Instance.PlayOnShot(clickAudioClip, Vector3.zero, 1, false);
        window.SelectFacedeMenusTab(this);
    }
    /// <summary>
    /// 选中
    /// </summary>
    public void Select()
    {
        iconImg.color = colors[1];
        tabFocus.enabled = true;
    }
    /// <summary>
    /// 取消选中
    /// </summary>
    public void UnSelect()
    {
        iconImg.color = colors[0];
        tabFocus.enabled = false;
    }
}
