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
            childTrack.Init(menuParent, childTracksList.Count, null, DeleteChildTrack);
            childTracksList.Add(childTrack);
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

        private Action<ChildTrack> deleteAction;
        private int index;

        public void Init(VisualElement menuParent, int index, VisualElement trackParent, Action<ChildTrack> deleteAction)
        {
            this.menuParent = menuParent;
            this.trackParent = trackParent;
            this.deleteAction = deleteAction;

            menuRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(menuItemAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体
            menuParent.Add(menuRoot);                                                                                             //contentRoot = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(trackAssetPath).Instantiate().Query().ToList()[1];//不要容器，直接持有目标物体

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
