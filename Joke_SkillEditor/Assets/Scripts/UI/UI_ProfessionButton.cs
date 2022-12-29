using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;
/// <summary>
/// 职业选择按钮
/// </summary>
public class UI_ProfessionButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image icon;
    [SerializeField] Image selectFrame;
    [SerializeField] Text nameText;
    [SerializeField] AudioClip clickAudioClip;

    public ProfessionType ProfessionType { get; private set; }
    private UI_CreateCharacterWindow window;

    private static Color[] colors;
    static UI_ProfessionButton()
    {
        colors = new Color[2];
        colors[0] = Color.white;
        colors[1] = new Color(0.964f, 0.882f, 0.611f);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(UI_CreateCharacterWindow window,ProfessionType professionType)
    {
        button.onClick.AddListener(ButtonClick);
        this.window = window;
        this.ProfessionType = professionType;
        UnSelect(); // 默认没有选中
    }

    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    private void ButtonClick()
    {
        // 告诉窗口，当前按钮代表的职业被玩家选择了
        AudioManager.Instance.PlayOnShot(clickAudioClip, Vector3.zero, 1, false);
        window.SelectProfession_Button(this);
    }

    /// <summary>
    /// 选中
    /// </summary>
    public void Select()
    {
        icon.color = colors[1];
        nameText.color = colors[1];
        selectFrame.enabled = true;
    }
    /// <summary>
    /// 取消选中
    /// </summary>
    public void UnSelect()
    {
        icon.color = colors[0];
        nameText.color = colors[0];
        selectFrame.enabled = false;
    }
}
