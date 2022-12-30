using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using System;

public class SkillEditorWindows : EditorWindow
{
    [MenuItem("SkillEditor/SkillEditorWindows")]
    public static void ShowExample()
    {
        SkillEditorWindows wnd = GetWindow<SkillEditorWindows>();
        wnd.titleContent = new GUIContent("技能编辑器");
    }

    private VisualElement root;
    public void CreateGUI()
    {
        root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SkillEditor/Editor/EditorWindows/SkillEditorWindows.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        InitTopMenu();
        InitTimeShaft();
        InitConsole();
        InitContent();

        if (skillConfig != null)
        {
            SkillConfigObjectField.value = skillConfig;
        }
        else
        {
            CurrentFrameCount = 100;
        }

        CurrentSelectFrameIndex = 0;
    }


    #region TopMenu 
    private const string skillEditorScenePath = "Assets/SkillEditor/SkillEditorScene.unity";
    private const string PreviewCharacterParentPath = "PreviewCharacterRoot";
    private string oldScenePath;

    private Button LoadEditorSceneButton;
    private Button LoadOldSceneButton;
    private Button SkillBasicButton;

    private ObjectField PreviewCharacterPrefabObjectField;
    private ObjectField SkillConfigObjectField;
    private GameObject currentPreviewCharacterObj;

    private void InitTopMenu()
    {
        LoadEditorSceneButton = root.Q<Button>(nameof(LoadEditorSceneButton));
        LoadOldSceneButton = root.Q<Button>(nameof(LoadOldSceneButton));
        SkillBasicButton = root.Q<Button>(nameof(SkillBasicButton));

        PreviewCharacterPrefabObjectField = root.Q<ObjectField>(nameof(PreviewCharacterPrefabObjectField));
        SkillConfigObjectField = root.Q<ObjectField>(nameof(SkillConfigObjectField));

        LoadEditorSceneButton.clicked += LoadEditorSceneButtonClick;
        LoadOldSceneButton.clicked += LoadOldSceneButtonClick;
        SkillBasicButton.clicked += SkillBasicButtonClick;

        PreviewCharacterPrefabObjectField.RegisterValueChangedCallback(PreviewCharacterPrefabObjectFieldChanged);
        SkillConfigObjectField.RegisterValueChangedCallback(SkillConfigObjectFieldChanged);
    }



    /// <summary>
    /// 加载编辑器场景
    /// </summary>
    private void LoadEditorSceneButtonClick()
    {
        string currentpath = EditorSceneManager.GetActiveScene().path;
        if (currentpath == skillEditorScenePath) return;

        oldScenePath = currentpath;
        EditorSceneManager.OpenScene(skillEditorScenePath);
    }

    /// <summary>
    /// 回归旧场景
    /// </summary>
    private void LoadOldSceneButtonClick()
    {
        if (!string.IsNullOrEmpty(oldScenePath))
        {
            string currentpath = EditorSceneManager.GetActiveScene().path;
            if (currentpath == oldScenePath) return;

            EditorSceneManager.OpenScene(oldScenePath);
        }
        else
        {
            Debug.LogWarning("场景不存在");
        }
    }

    /// <summary>
    /// 查看技能基本信息
    /// </summary>
    private void SkillBasicButtonClick()
    {
        if (skillConfig != null)
        {
            Selection.activeObject = skillConfig;
        }
    }

    /// <summary>
    /// 角色预制体修改
    /// </summary>
    /// <param name="evt"></param>
    private void PreviewCharacterPrefabObjectFieldChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        string currentpath = EditorSceneManager.GetActiveScene().path;
        if (currentpath != skillEditorScenePath) return;

        //销毁旧的
        if (currentPreviewCharacterObj != null) DestroyImmediate(currentPreviewCharacterObj);


        Transform parent = GameObject.Find(PreviewCharacterParentPath).transform;
        if (parent != null && parent.childCount > 0)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }

        if (evt.newValue != null)
        {
            currentPreviewCharacterObj = Instantiate(evt.newValue as GameObject, Vector3.zero, Quaternion.identity, parent);
            currentPreviewCharacterObj.transform.localEulerAngles = Vector3.zero;
        }

    }

    /// <summary>
    /// 技能配置修改
    /// </summary>
    /// <param name="evt"></param>
    private void SkillConfigObjectFieldChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        skillConfig = evt.newValue as SkillConfig;
        CurrentFrameCount = skillConfig.FrameCount;
    }

    #endregion Config

    #region TimeShaft
    private IMGUIContainer timeShaft;//时间轴容器
    private IMGUIContainer selectLine;// 
    private VisualElement contentContainer;// ScrollView 容器,方便得出  ScrollView 往左往右拽的尺寸坐标 
    private VisualElement contentViewPort; //时间线的显示区域  

    private int currentSelectFrameIndex = -1;
    /// <summary>
    /// 鼠标位置+下方容器的位置
    /// </summary>
    private int CurrentSelectFrameIndex
    {
        get => currentSelectFrameIndex;
        set
        {
            if (currentSelectFrameIndex == value) return;

            //如果超出范围，更新最大帧
            if (value > CurrentFrameCount) CurrentFrameCount = value;

            currentSelectFrameIndex = Mathf.Clamp(value, 0, CurrentFrameCount);
            CurrentFrameTextField.value = currentSelectFrameIndex;
            UpdateTimerShaftView();
        }
    }

    private int currentFrameCount;
    public int CurrentFrameCount
    {
        get => currentFrameCount;
        set
        {
            if (currentFrameCount == value) return;

            currentFrameCount = value;
            FrameCountTextField.value = currentFrameCount;

            //同步给 skillConfig
            if (skillConfig != null)
            {
                skillConfig.FrameCount = currentFrameCount;
                SaveConfig();
            }

            //Content 区域的尺寸变化
            UpdateContentSize();
        }
    }

    /// <summary>
    /// 当前内容区域的偏移坐标
    /// </summary>
    private float contentOffsetPos { get => Mathf.Abs(contentContainer.transform.position.x); }
    /// <summary>
    /// 当前帧在时间轴的像素坐标位置（鼠标位置+下方容器的移动位置）
    /// </summary>
    private float currentSelectFramePos { get => CurrentSelectFrameIndex * skillEditorConfig.frameUnitWidth; }


    private bool timeShaftIsMouseEnter = false;

    private void InitTimeShaft()
    {
        ScrollView MainContentView = root.Q<ScrollView>("MainContentView");
        contentContainer = MainContentView.Q<VisualElement>("unity-content-container");
        contentViewPort = MainContentView.Q<VisualElement>("unity-content-viewport");

        timeShaft = root.Q<IMGUIContainer>("TimeShaft");
        selectLine = root.Q<IMGUIContainer>("SelectLine");


        timeShaft.onGUIHandler = DrawTimeShaft;
        timeShaft.RegisterCallback<WheelEvent>(TimeShaftWheel);
        timeShaft.RegisterCallback<MouseDownEvent>(TimeShaftMouseDown);
        timeShaft.RegisterCallback<MouseMoveEvent>(TimeShaftMouseMove);
        timeShaft.RegisterCallback<MouseUpEvent>(TimeShaftMouseUp);
        timeShaft.RegisterCallback<MouseOutEvent>(TimeShaftMouseOut);

        selectLine.onGUIHandler = DrawSelectLine;
    }

    private void DrawTimeShaft()
    {
        Handles.BeginGUI();
        Handles.color = Color.white;
        Rect rect = timeShaft.contentRect; //时间轴的尺寸

        //起始索引
        int index = Mathf.CeilToInt(contentOffsetPos / skillEditorConfig.frameUnitWidth);
        //计算绘制起点的偏移
        float startOffset = 0;
        //10-(98 % 10)
        //=10-8=2
        if (index > 0) startOffset = skillEditorConfig.frameUnitWidth - (contentOffsetPos % skillEditorConfig.frameUnitWidth);

        int tickStep = SkillEditorConfig.MaxFrameWidthLV + 1 - (skillEditorConfig.frameUnitWidth / SkillEditorConfig.StandframeUnitWidth);
        //tickStep = 10+1-(100/10)=1
        //tickStep = 11-9=2
        //tickStep = 11-8=3
        //tickStep = 11-1=10

        tickStep = Mathf.Clamp(tickStep / 2, 1, SkillEditorConfig.MaxFrameWidthLV);

        for (float i = startOffset; i < rect.width; i += skillEditorConfig.frameUnitWidth)
        {
            //绘制长线条、文本
            if (index % tickStep == 0)
            {
                Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
                string indexStr = index.ToString();
                GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);
            }
            else
            {
                Handles.DrawLine(new Vector3(i, rect.height - 5), new Vector3(i, rect.height));
            }

            index += 1;
        }
        Handles.EndGUI();
    }

    private void TimeShaftWheel(WheelEvent evt)
    {
        int delta = (int)evt.delta.y;
        skillEditorConfig.frameUnitWidth = Mathf.Clamp(skillEditorConfig.frameUnitWidth - delta,
            SkillEditorConfig.StandframeUnitWidth, SkillEditorConfig.MaxFrameWidthLV * SkillEditorConfig.StandframeUnitWidth);

        UpdateTimerShaftView();
        UpdateContentSize();
    }


    private void TimeShaftMouseDown(MouseDownEvent evt)
    {
        //让选中线位置卡在帧的位置上
        timeShaftIsMouseEnter = true;
        CurrentSelectFrameIndex = GetFrameIndexByMousePos(evt.localMousePosition.x);

    }
    private void TimeShaftMouseMove(MouseMoveEvent evt)
    {
        if (timeShaftIsMouseEnter)
        {
            CurrentSelectFrameIndex = GetFrameIndexByMousePos(evt.localMousePosition.x);
        }
    }

    private void TimeShaftMouseUp(MouseUpEvent evt)
    {
        timeShaftIsMouseEnter = false;
    }

    private void TimeShaftMouseOut(MouseOutEvent evt)
    {
        timeShaftIsMouseEnter = false;
    }

    /// <summary>
    /// 根据鼠标坐标获取帧像素索引；
    /// 鼠标位置+下方容器的位置
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private int GetFrameIndexByMousePos(float x)
    {
        float pos = x + contentOffsetPos;
        return Mathf.RoundToInt(pos / skillEditorConfig.frameUnitWidth);
    }


    private void DrawSelectLine()
    {
        //判断当前选中帧是否在视图范围内
        if (currentSelectFramePos >= contentOffsetPos)
        {
            Handles.BeginGUI();
            Handles.color = Color.white;
            float x = currentSelectFramePos - contentOffsetPos;
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, contentViewPort.contentRect.height + timeShaft.contentRect.height));
            Handles.EndGUI();
        }
    }

    private void UpdateTimerShaftView()
    {
        timeShaft.MarkDirtyLayout();//标记为需要立刻重新绘制的
        selectLine.MarkDirtyLayout();//标记为需要立刻重新绘制的
    }

    #endregion

    #region Consle
    private Button PreviouFrameButton;
    private Button PlayButton;
    private Button NextFrameButton;
    private IntegerField CurrentFrameTextField;
    private IntegerField FrameCountTextField;

    private void InitConsole()
    {
        PreviouFrameButton = root.Q<Button>(nameof(PreviouFrameButton));
        PlayButton = root.Q<Button>(nameof(PlayButton));
        NextFrameButton = root.Q<Button>(nameof(NextFrameButton));

        CurrentFrameTextField = root.Q<IntegerField>(nameof(CurrentFrameTextField));
        FrameCountTextField = root.Q<IntegerField>(nameof(FrameCountTextField));

        PreviouFrameButton.clicked += PreviouFrameButtonClicked;
        PlayButton.clicked += PlayButtonClicked;
        NextFrameButton.clicked += NextFrameButtonClicked;

        CurrentFrameTextField.RegisterValueChangedCallback(CurrentFrameTextFieldValueChanged);
        FrameCountTextField.RegisterValueChangedCallback(FrameCountTextFieldValueChanged);

    }

    private void PreviouFrameButtonClicked()
    {
        CurrentSelectFrameIndex -= 1;
    }

    private void PlayButtonClicked()
    {
    }

    private void NextFrameButtonClicked()
    {
        CurrentSelectFrameIndex += 1;
    }

    private void CurrentFrameTextFieldValueChanged(ChangeEvent<int> evt)
    {
        CurrentSelectFrameIndex = evt.newValue;
    }
    private void FrameCountTextFieldValueChanged(ChangeEvent<int> evt)
    {
        CurrentFrameCount = evt.newValue;
    }


    #endregion


    #region config
    private SkillConfig skillConfig;
    private SkillEditorConfig skillEditorConfig = new SkillEditorConfig();

    private void SaveConfig()
    {
        if (skillConfig != null)
        {
            EditorUtility.SetDirty(skillConfig);
            AssetDatabase.SaveAssetIfDirty(skillConfig);
        }
    }

    #endregion


    #region  Track
    private VisualElement trackMenuParent;
    private VisualElement ContentListView;

    private void InitContent()
    {
        trackMenuParent = root.Q<VisualElement>("TrackMenu");
        ContentListView = root.Q<VisualElement>(nameof(ContentListView));
        UpdateContentSize();
        InitAnimationTrack();
    }

    /// <summary>
    /// Content 区域的尺寸变化
    /// </summary>
    private void UpdateContentSize()
    {
        ContentListView.style.width = skillEditorConfig.frameUnitWidth * CurrentFrameCount;
    }

    private void InitAnimationTrack()
    {
        AnimationTrack animationTrack = new AnimationTrack();
        animationTrack.Init(trackMenuParent, ContentListView);
    }

    #endregion


}

public class SkillEditorConfig
{
    /// <summary>
    /// 每帧的标准单位像素刻度
    /// </summary>
    public const int StandframeUnitWidth = 10;

    /// <summary>
    /// 分10级
    /// </summary>
    public const int MaxFrameWidthLV = 10;

    /// <summary>
    /// 当前的帧单位刻度（受缩放而变化）
    /// </summary>
    public int frameUnitWidth = 10;



}