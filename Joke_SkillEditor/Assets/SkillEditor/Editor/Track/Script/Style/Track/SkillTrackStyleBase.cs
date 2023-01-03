using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class SkillTrackStyleBase
{
    public Label titleLabel;
    #region �������ڵ㣨�ŵ�˭���棩
    public VisualElement menuParent;
    public VisualElement contentParent;
    #endregion
    #region �������ڵ㣨���Լ���
    public VisualElement menuRoot;
    public VisualElement contentRoot;
    #endregion

    public virtual void AddItem(VisualElement ve)
    {
        contentRoot.Add(ve);
    }

    public virtual void DeleteItem(VisualElement ve)
    {
        contentRoot.Remove(ve);
    }





}