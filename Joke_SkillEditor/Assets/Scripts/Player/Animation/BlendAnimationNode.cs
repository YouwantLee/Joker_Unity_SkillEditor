using JKFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[Pool]
public class BlendAnimationNode : AnimationNodeBase
{
    private AnimationMixerPlayable blendMixer;
    private List<AnimationClipPlayable> blendClipPlayableList = new List<AnimationClipPlayable>(10);

    public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, List<AnimationClip> clips, float speed, int inputPort)
    {
        blendMixer = AnimationMixerPlayable.Create(graph, clips.Count);
        graph.Connect(blendMixer, 0, outputMixer, inputPort);
        this.InputPort = inputPort;
        for (int i = 0; i < clips.Count; i++)
        {
            CreateAndConnectBlendPlayable(graph, clips[i],i,speed);
        }
    }

    public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, AnimationClip clip1, AnimationClip clip2, float speed, int inputPort)
    {
        blendMixer = AnimationMixerPlayable.Create(graph, 2);
        graph.Connect(blendMixer, 0, outputMixer, inputPort);
        this.InputPort = inputPort;
        CreateAndConnectBlendPlayable(graph, clip1, 0, speed);
        CreateAndConnectBlendPlayable(graph, clip2, 1, speed);
    }

    private AnimationClipPlayable CreateAndConnectBlendPlayable(PlayableGraph graph,AnimationClip clip, int index, float speed)
    {
        AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, clip);
        clipPlayable.SetSpeed(speed);
        blendClipPlayableList.Add(clipPlayable);
        graph.Connect(clipPlayable, 0, blendMixer, index);
        return clipPlayable;
    }

    public void SetBlendWeight(List<float> weightList)
    {
        for (int i = 0; i < blendClipPlayableList.Count; i++)
        {
            blendMixer.SetInputWeight(i, weightList[i]);
        }
    }
    public void SetBlendWeight(float clip1Weight)
    {
        blendMixer.SetInputWeight(0, clip1Weight);
        blendMixer.SetInputWeight(1, 1 - clip1Weight);
    }

    public override void SetSpeed(float speed)
    {
        for (int i = 0; i < blendClipPlayableList.Count; i++)
        {
            blendClipPlayableList[i].SetSpeed(speed);
        }
    }
    public override void PushPool()
    {
        blendClipPlayableList.Clear();
        base.PushPool();
    }
}
