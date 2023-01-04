using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioTrack : SkillTrackBase
{

    private SkillMultiLineTrackStyle trackStyle;

    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillMultiLineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "“Ù–ß≈‰÷√", ChectAddChildTrack);
        //trackStyle.contentRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        //trackStyle.contentRoot.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);

        //ResetView();
    }

    private bool ChectAddChildTrack()
    {

        return true;
    }

    public override void Destory()
    {
        trackStyle.Destory();
    }



}
