using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class TrackItemBase
{
    public abstract void Select();

    public abstract void OnSelect();
    public abstract void OnUnSelect();

}

public abstract class TrackItemBase<T> : TrackItemBase where T : SkillTrackBase
{
    protected T track;
    protected Color normalColor = new Color(0.388f, 0.850f, 0.905f, 0.5f);
    protected Color selectColor = new Color(0.388f, 0.850f, 0.905f, 1f);
    public Label root { get; protected set; }

    protected float frameUnitWidth;

    protected int frameIndex;
    public int FrameIndex { get => frameIndex; }

    public override void Select()
    {
        SkillEditorWindows.Instance.ShowTrackItemOnInspector(this, track);
    }

    public override void OnSelect()
    {
        root.style.backgroundColor = selectColor;
    }

    public override void OnUnSelect()
    {
        root.style.backgroundColor = normalColor;
    }
}
