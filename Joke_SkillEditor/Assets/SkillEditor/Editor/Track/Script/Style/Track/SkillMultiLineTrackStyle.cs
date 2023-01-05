using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillMultiLineTrackStyle : SkillTrackStyleBase
{
    #region 常量
    private const string menuAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackMenu.uxml";
    private const string trackAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackContent.uxml";
    private const float headHeight = 35;//5是间距
    private const float itemHeight = 32;//2是底部外边距

    #endregion


    private Func<bool> addChildTrackFunc;
    private Func<int, bool> deleteChildTrackFunc;

    private VisualElement menuItemParent;//子轨道的菜单父物体
    private List<ChildTrack> childTracksList = new List<ChildTrack>();


    public void Init(VisualElement menuParent, VisualElement contentParent, string title, Func<bool> addChildTrackFunc, Func<int, bool> deleteChildTrackFunc)
    {
        this.menuParent = menuParent;
        this.contentParent = contentParent;
        this.addChildTrackFunc = addChildTrackFunc;
        this.deleteChildTrackFunc = deleteChildTrackFunc;

        menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menuAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
        contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
        menuParent.Add(menuRoot);
        contentParent.Add(contentRoot);

        titleLabel = menuRoot.Q<Label>("Title");
        titleLabel.text = title;

        menuItemParent = menuRoot.Q<VisualElement>("TrackMenuList");

        //添加子轨道的按钮
        Button addButton = menuRoot.Q<Button>("AddButton");
        addButton.clicked += AddButtonClicked;

        UpdateSize();
    }

    private void UpdateSize()
    {
        float height = headHeight + (childTracksList.Count * itemHeight);
        contentRoot.style.height = height;
        menuRoot.style.height = height;
    }


    //添加子轨道
    private void AddButtonClicked()
    {
        if (addChildTrackFunc == null) return;

        //由上级具体轨道类来判断能不能添加
        if (addChildTrackFunc())
        {
            ChildTrack childTrack = new ChildTrack();
            childTrack.Init(menuItemParent, childTracksList.Count, contentRoot, DeleteChildTrack);
            childTracksList.Add(childTrack);
            UpdateSize();
        }
    }

    //删除子轨道
    private void DeleteChildTrack(ChildTrack childTrack)
    {
        if (deleteChildTrackFunc == null) return;
        int index = childTrack.GetIndex();
        if (deleteChildTrackFunc(index))
        {
            childTrack.Destory();
            childTracksList.RemoveAt(index);
            //所有的子轨道都需要更新一下索引
            UpdateChilds(index);
            UpdateSize();
        }
    }

    private void UpdateChilds(int startIndex = 0)
    {
        for (int i = startIndex; i < childTracksList.Count; i++)
        {
            childTracksList[i].SetIndex(i);
        }
    }


    /// <summary>
    /// 多行轨道的子轨道
    /// </summary>
    public class ChildTrack
    {
        private const string childTrackMenuAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackMenuItem.uxml";
        private const string childTrackContentAssetPath = "Assets/SkillEditor/Editor/Track/Assets/MultiLineTrackStyle/MultiLineTrackContentItem.uxml";

        public Label titleLabel;
        #region 自身根节点（我自己）
        public VisualElement menuRoot;
        public VisualElement trackRoot;
        #endregion
        #region 自身父节点（放到谁上面）
        public VisualElement menuParent;
        public VisualElement trackParent;
        #endregion

        private Action<ChildTrack> deleteAction;
        private int index;

        public void Init(VisualElement menuParent, int index, VisualElement trackParent, Action<ChildTrack> deleteAction)
        {
            this.menuParent = menuParent;
            this.trackParent = trackParent;
            this.deleteAction = deleteAction;

            menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(childTrackMenuAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
            trackRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(childTrackContentAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
            menuParent.Add(menuRoot);
            trackParent.Add(trackRoot);

            Button deleteButton = menuRoot.Q<Button>("DeleteButton");
            deleteButton.clicked += () => deleteAction(this);

            SetIndex(index);
        }

        public int GetIndex()
        {
            return index;
        }

        public void SetIndex(int index)
        {
            this.index = index;
            float height = 0;
            Vector3 menuPos = menuRoot.transform.position;
            height = index * itemHeight;
            menuPos.y = height;
            menuRoot.transform.position = menuPos;

            Vector3 trackPos = trackRoot.transform.position;
            height = index * itemHeight + headHeight;
            trackPos.y = height;
            trackRoot.transform.position = trackPos;
        }


        public virtual void AddItem(VisualElement ve)
        {
            trackRoot.Add(ve);
        }

        public virtual void DeleteItem(VisualElement ve)
        {
            trackRoot.Remove(ve);
        }

        public virtual void Destory()
        {
            if (menuRoot != null) menuParent.Remove(menuRoot);
            if (trackRoot != null) trackParent.Remove(trackRoot);
        }










    }

}
