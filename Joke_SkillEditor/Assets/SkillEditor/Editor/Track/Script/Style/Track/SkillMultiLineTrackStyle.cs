using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillMultiLineTrackStyle : SkillTrackStyleBase
{
    private const string menuAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackMenu.uxml";
    private const string trackAssetPath = "Assets/SkillEditor/Editor/Track/Assets/SingleLineTrackStyle/SingleLineTrackContent.uxml";
    private Func<bool> addChildTrackFunc;

    private VisualElement menuItemParent;//子轨道的菜单父物体


    public void Init(VisualElement menuParent, VisualElement contentParent, string title, Func<bool> addChildTrackFunc)
    {
        this.menuParent = menuParent;
        this.contentParent = contentParent;
        this.addChildTrackFunc = addChildTrackFunc;

        menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menuAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
        //contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
        menuParent.Add(menuRoot);
        //contentParent.Add(contentRoot);

        titleLabel = menuRoot.Q<Label>("Title");
        titleLabel.text = title;

        menuItemParent = menuRoot.Q<VisualElement>("TrackMenuList");

        //添加子轨道的按钮
        Button addButton = menuRoot.Q<Button>("AddButton");
        addButton.clicked += AddButtonClicked;
    }

    //添加子轨道
    private void AddButtonClicked()
    {
        if (addChildTrackFunc == null) return;

        //由上级具体轨道类来判断能不能添加
        if (addChildTrackFunc())
        {
            ChildTrack childTrack = new ChildTrack();
            childTrack.Init(menuParent, null);
        }

    }


    /// <summary>
    /// 多行轨道的子轨道
    /// </summary>
    public class ChildTrack
    {
        private const string menuItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackMenuItem.uxml";
        private const string trackItemAssetPath = "Assets/SkillEditor/Editor/Track/Assets/SingleLineTrackStyle/SingleLineTrackContent.uxml";

        public Label titleLabel;
        #region 自身根节点（我自己）
        public VisualElement menuRoot;
        public VisualElement contentRoot;
        #endregion
        #region 自身父节点（放到谁上面）
        public VisualElement menuParent;
        public VisualElement trackParent;
        #endregion

        public void Init(VisualElement menuParent, VisualElement trackParent)
        {
            this.menuParent = menuParent;
            this.trackParent = trackParent;

            menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menuItemAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
            menuParent.Add(menuRoot);                                                                                             //contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体

        }

        public virtual void AddItem(VisualElement ve)
        {
            contentRoot.Add(ve);
        }

        public virtual void DeleteItem(VisualElement ve)
        {
            contentRoot.Remove(ve);
        }

        public virtual void Destory()
        {
            if (menuRoot != null) menuParent.Remove(menuRoot);
            if (contentRoot != null) trackParent.Remove(contentRoot);
        }










    }

}
