using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JKFrame;
using UnityEngine.EventSystems;
using System;

[UIElement(false, "UI_CreateCharacterWindow",2)]
public class UI_CreateCharacterWindow : UI_WindowBase
{
    // 模型交互Image
    [SerializeField] Image modelTouchImage;
    // 部位名称Text
    [SerializeField] Text partNameText;
    // 左箭头按钮
    [SerializeField] Button leftArrowButton;
    // 右箭头按钮
    [SerializeField] Button rightArrowButton;

    [SerializeField] Slider sizeSlider;
    [SerializeField] Slider heightSlider;
    [SerializeField] Button color1Button;
    [SerializeField] Button color2Button;

    [SerializeField] Button backButton;
    [SerializeField] Button submitButton;

    // 所有的职业按钮
    [SerializeField] UI_ProfessionButton[] professionButtons;
    // 所有的外观类型按钮
    [SerializeField] UI_FacadeMenus_Tab[] facadeMenus_Tabs;
    [SerializeField] AudioClip arrowClickAudioClip;

    // 当前选择的职业按钮
    private UI_ProfessionButton currentProfessionButton;
    // 当前选择的外观类型
    private UI_FacadeMenus_Tab currentFacadeMenus_Tab;

    // 自定义角色的数据
    private CustomCharacterData customCharacterData => DataManager.CustomCharacterData;
    private ProjectConfig projectConfig;
    // 玩家当前每一个部位 选择的在projectConfig中第几个配置
    private Dictionary<int, int> characterConfigIndexDic;

    public override void Init()
    {
        // 获取配置
        projectConfig = ConfigManager.Instance.GetConfig<ProjectConfig>(ConfigTool.ProjectConfigName);
        characterConfigIndexDic = new Dictionary<int, int>(3);
        characterConfigIndexDic.Add((int)CharacterPartType.Face, 0);
        characterConfigIndexDic.Add((int)CharacterPartType.Hair, 0);
        characterConfigIndexDic.Add((int)CharacterPartType.Cloth, 0);

        // 绑定modelTouchImage的拖拽事件
        modelTouchImage.OnDrag(ModelTouchImageDrag,6);
        leftArrowButton.onClick.AddListener(LeftArrowButtonClick);
        rightArrowButton.onClick.AddListener(RightArrowButtonClick);
        sizeSlider.onValueChanged.AddListener(OnSizeSliderValueChanged);
        heightSlider.onValueChanged.AddListener(OnHeightSliderValueChanged);

        // 绑定颜色选项按钮的点击事件
        color1Button.onClick.AddListener(Color1ButtonClick);
        color2Button.onClick.AddListener(Color2ButtonClick);

        // 初始化外观类型菜单
        facadeMenus_Tabs[0].Init(this, CharacterPartType.Face);
        facadeMenus_Tabs[1].Init(this, CharacterPartType.Hair);
        facadeMenus_Tabs[2].Init(this, CharacterPartType.Cloth);
        // 选择默认外观类型 (脸部)
        SelectFacedeMenusTab(facadeMenus_Tabs[0]);
        // 应用默认的部位
        SetCharacterPart(CharacterPartType.Face, customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Face].Index, true, true);
        SetCharacterPart(CharacterPartType.Hair, customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Hair].Index, false, true);
        SetCharacterPart(CharacterPartType.Cloth, customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Cloth].Index, false, true);

        // 初始化职业按钮
        professionButtons[0].Init(this, ProfessionType.Warrior);
        professionButtons[1].Init(this, ProfessionType.Assassin);
        professionButtons[2].Init(this, ProfessionType.Archer);
        professionButtons[3].Init(this, ProfessionType.Tanke);
        // 默认选择一个职业 (战士)
        SelectProfession_Button(professionButtons[0]);

        backButton.onClick.AddListener(BackButtonClick);
        submitButton.onClick.AddListener(SubmitButtonClick);
        
    }

    private void BackButtonClick()
    {
        Close();
        SceneManager.LoadScene("Menu");
    }
    private void SubmitButtonClick()
    { 
        Close();
        // 保存数据
        DataManager.SaveCustomCharacterData();
        // 进入游戏场景
        SceneManager.LoadScene("Game");
    }


    // 最后一次坐标
    float lastPosXOnDragModel = 0;
    /// <summary>
    /// 当模型交互区域拖拽时的回调
    /// </summary>
    private void ModelTouchImageDrag(PointerEventData eventData, object[] arg2)
    {
        float offset = eventData.position.x - lastPosXOnDragModel;
        lastPosXOnDragModel = eventData.position.x;
        CharacterCreator.Instance.RotateCharacter(new Vector3(0, -offset * Time.deltaTime * 60f, 0));
    }

    /// <summary>
    /// 选择职业按钮
    /// </summary>
    public void SelectProfession_Button(UI_ProfessionButton newButton)
    {
        // 如果相等则啥也不干
        if (currentProfessionButton == newButton) return;

        // 处理之前的职业按钮
        if (currentProfessionButton!=null) currentProfessionButton.UnSelect();

        // 处理新按钮
        newButton.Select();
        currentProfessionButton = newButton;
        SelectProfession(newButton.ProfessionType);
    }

    /// <summary>
    /// 选择职业
    /// </summary>
    private void SelectProfession(ProfessionType professionType)
    {
        // 处理实际的职业切换逻辑
        CharacterCreator.Instance.SetProfession(professionType);

        // 检查三个部位是否有不支持这个职业的情况
        CharacterPartConfigBase partConfig = GetCurrentCharacterPartConfig(CharacterPartType.Face);
        // TODO:这里存在资源的卸载问题
        if (!partConfig.ProfessionTypes.Contains(professionType))
        {
            // 切换脸部部位
            SetNextChracterPart(false, CharacterPartType.Face==currentFacadeMenus_Tab.CharacterPartType, CharacterPartType.Face);
        }

        partConfig = GetCurrentCharacterPartConfig(CharacterPartType.Hair);
        if (!partConfig.ProfessionTypes.Contains(professionType))
        {
            // 切换头发部位
            SetNextChracterPart(false, CharacterPartType.Hair == currentFacadeMenus_Tab.CharacterPartType, CharacterPartType.Hair);
        }

        partConfig = GetCurrentCharacterPartConfig(CharacterPartType.Cloth);
        if (!partConfig.ProfessionTypes.Contains(professionType))
        {
            // 切换上衣部位
            SetNextChracterPart(false, CharacterPartType.Cloth == currentFacadeMenus_Tab.CharacterPartType, CharacterPartType.Cloth);
        }
    }

    /// <summary>
    /// 选择外观菜单
    /// </summary>
    public void SelectFacedeMenusTab(UI_FacadeMenus_Tab newTab)
    {
        if (currentFacadeMenus_Tab!=null)
        {
            currentFacadeMenus_Tab.UnSelect();
        }
        newTab.Select();
        currentFacadeMenus_Tab = newTab;
        int currIndex = characterConfigIndexDic[(int)currentFacadeMenus_Tab.CharacterPartType];
        // 刷新界面
        SetCharacterPart(currentFacadeMenus_Tab.CharacterPartType, projectConfig.CustomCharacterPartConfigIDDic[currentFacadeMenus_Tab.CharacterPartType][currIndex], true,false);
    }


    /// <summary>
    /// 设置具体的部位
    /// </summary>
    public void SetCharacterPart(CharacterPartType partType,int configIndex,bool updateUIView = false,bool updateCharacterView = false)
    {
        // 获取配置文件 
        // 这个配置文件的资源释放时机由Player_View来决定
        CharacterPartConfigBase partConfig = ConfigTool.LoadCharacterPartConfig(partType, configIndex);

        // 更新UI
        if (updateUIView)
        {
            partNameText.text = partConfig.Name;
            switch (partType)
            {
                case CharacterPartType.Face:
                    // 高度
                    heightSlider.transform.parent.gameObject.SetActive(true);
                    heightSlider.value = customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Face].Height;
                    heightSlider.minValue = 0;
                    heightSlider.maxValue = 0.1f;
              
                    // 尺寸
                    sizeSlider.transform.parent.gameObject.SetActive(true);
                    sizeSlider.value = customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Face].Size;
                    sizeSlider.minValue = 0.9f;
                    sizeSlider.maxValue = 1.1f;
            
                    // 隐藏颜色选项
                    color1Button.gameObject.SetActive(false);
                    color2Button.gameObject.SetActive(false);
                    break;
                case CharacterPartType.Hair:
                    heightSlider.transform.parent.gameObject.SetActive(false);
                    sizeSlider.transform.parent.gameObject.SetActive(false);
                    color2Button.gameObject.SetActive(false);

                    // 根据配置的有效性来决定是否隐藏颜色
                    if ((partConfig as HairConfig).ColorIndex != -1) 
                    {
                        color1Button.gameObject.SetActive(true);
                        Color color = customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Hair].Color1.ConverToUnityColor();
                        // 让颜色按钮的图片和当前配置一样
                        color1Button.image.color = new Color(color.r, color.g, color.b, 0.6f);
                    }
                    else color1Button.gameObject.SetActive(false);

                    break;
                case CharacterPartType.Cloth:
                    heightSlider.transform.parent.gameObject.SetActive(false);
                    sizeSlider.transform.parent.gameObject.SetActive(false);

                    // 根据配置的有效性来决定是否隐藏颜色
                    if ((partConfig as ClothConfig).ColorIndex1 != -1)
                    {
                        color1Button.gameObject.SetActive(true);
                        Color color = customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Cloth].Color1.ConverToUnityColor();
                        // 让颜色按钮的图片和当前配置一样
                        color1Button.image.color = new Color(color.r, color.g, color.b, 0.6f);
                    }
                    else color1Button.gameObject.SetActive(false);

                    // 根据配置的有效性来决定是否隐藏颜色
                    if ((partConfig as ClothConfig).ColorIndex2 != -1)
                    {
                        color2Button.gameObject.SetActive(true);
                        Color color = customCharacterData.CustomPartDataDic.Dictionary[(int)CharacterPartType.Cloth].Color2.ConverToUnityColor();
                        // 让颜色按钮的图片和当前配置一样
                        color2Button.image.color = new Color(color.r, color.g, color.b, 0.6f);
                    }
                    else color2Button.gameObject.SetActive(false);
                    break;
            }
        }
        // 让角色修改模型
        CharacterCreator.Instance.SetPart(partConfig, updateCharacterView);

        // 保存数据
        customCharacterData.CustomPartDataDic.Dictionary[(int)partType].Index = configIndex;
    }

    private void LeftArrowButtonClick()
    {
        SetNextChracterPart(true,true, currentFacadeMenus_Tab.CharacterPartType);
    }
    private void RightArrowButtonClick()
    {
        SetNextChracterPart(false, true, currentFacadeMenus_Tab.CharacterPartType);
    }
    private void SetNextChracterPart(bool isLeft,bool updateUI, CharacterPartType currPartType)
    {
        // 当前的职业
        ProfessionType professionType = currentProfessionButton.ProfessionType;
        // 当前索引-在projectConfig中第几个配置
        int currIndex = characterConfigIndexDic[(int)currPartType];
        if (isLeft) currIndex -= 1;
        else currIndex += 1;

        // 到达边界了，直接设为另外一个边界
        if (currIndex < 0) currIndex = projectConfig.CustomCharacterPartConfigIDDic[currPartType].Count-1;
        else if(currIndex > projectConfig.CustomCharacterPartConfigIDDic[currPartType].Count-1) currIndex = 0;

        // 检查职业有效性
        // 通过currIndex知道当前是第几个配置，然后这个具体的配置ID，要去projectConfig里面去获取
        CharacterPartConfigBase partConfig = ConfigTool.LoadCharacterPartConfig(currPartType, projectConfig.CustomCharacterPartConfigIDDic[currPartType][currIndex]);
        while (!partConfig.ProfessionTypes.Contains(professionType))
        {
            if (isLeft) currIndex -= 1;
            else currIndex += 1;
            // 到达边界了，直接设为另外一个边界
            if (currIndex < 0) currIndex = projectConfig.CustomCharacterPartConfigIDDic[currPartType].Count-1;
            else if (currIndex > projectConfig.CustomCharacterPartConfigIDDic[currPartType].Count-1) currIndex = 0;
            // 释放资源
            ResManager.Release<CharacterPartConfigBase>(partConfig);
            partConfig = ConfigTool.LoadCharacterPartConfig(currPartType, projectConfig.CustomCharacterPartConfigIDDic[currPartType][currIndex]);
        }
        // 释放资源
        ResManager.Release<CharacterPartConfigBase>(partConfig);

        // 到这里职业有效了
        characterConfigIndexDic[(int)currPartType] = currIndex;
        SetCharacterPart(currPartType, projectConfig.CustomCharacterPartConfigIDDic[currPartType][currIndex], updateUI, true);
        AudioManager.Instance.PlayOnShot(arrowClickAudioClip, Vector3.zero, 1);
    }
    private void OnHeightSliderValueChanged(float height)
    {
        GetCharacterPartData().Height = height;
        CharacterCreator.Instance.SetHieght(currentFacadeMenus_Tab.CharacterPartType, height);
    }

    private void OnSizeSliderValueChanged(float size)
    {
        GetCharacterPartData().Size = size;
        CharacterCreator.Instance.SetSize(currentFacadeMenus_Tab.CharacterPartType, size);
    }

    private void Color1ButtonClick()
    {
        // 显示颜色选择器窗口
        UIManager.Instance.Show<UI_ColorSelectorWindow>().Init(OnColor1Seletec, color1Button.image.color);
    }

    private void Color2ButtonClick()
    {
        // 显示颜色选择器窗口
        UIManager.Instance.Show<UI_ColorSelectorWindow>().Init(OnColor2Seletec, color1Button.image.color);
    }

    // 玩家确定了第一个颜色按钮的值
    private void OnColor1Seletec(Color newColor)
    {
        GetCharacterPartData().Color1 = newColor.ConverToSerializationColor();
        // 传给角色那边修改颜色
        CharacterCreator.Instance.SetColor1(currentFacadeMenus_Tab.CharacterPartType, newColor);

        // 修改颜色按钮的颜色值
        color1Button.image.color = new Color(newColor.r, newColor.g, newColor.b, 0.6f);
    }


    // 玩家确定了第二个颜色按钮的值
    private void OnColor2Seletec(Color newColor)
    {
        GetCharacterPartData().Color2 = newColor.ConverToSerializationColor();

        // 传给角色那边修改颜色
        CharacterCreator.Instance.SetColor2(currentFacadeMenus_Tab.CharacterPartType, newColor);

        // 修改颜色按钮的颜色值
        color2Button.image.color = new Color(newColor.r, newColor.g, newColor.b, 0.6f);
    }

    // 获取当前角色部位配置
    private CharacterPartConfigBase GetCurrentCharacterPartConfig(CharacterPartType currPartType)
    {
        return CharacterCreator.Instance.GetCurrentPartConfig(currPartType);
    }

    // 获取当前角色部位数据
    private CustomCharacterPartData GetCharacterPartData()
    {
        // 确定当前是什么部位
        CharacterPartType currPartType = currentFacadeMenus_Tab.CharacterPartType;
        // 数据层面部位数据
        CustomCharacterPartData partData = customCharacterData.CustomPartDataDic.Dictionary[(int)currPartType];
        return partData;
    }

    public override void OnClose()
    {
        base.OnClose();
        // 释放自身资源
        ResManager.ReleaseInstance(gameObject);
    }
}
