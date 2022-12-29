using GraphVisualizer;
using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class Animation_Controller : MonoBehaviour
{
    [SerializeField] Animator animator;
    private PlayableGraph graph;
    private AnimationMixerPlayable mixer;

    private AnimationNodeBase previousNode; // 上一个节点
    private AnimationNodeBase currentNode;  // 当前节点
    private int inputPort0 = 0;
    private int inputPort1 = 1;


    private Coroutine transitionCoroutine;

    private float speed;
    public float Speed
    {
        get => speed;
        set {
            speed = value;
            currentNode.SetSpeed(speed);
        }
    }

    public void Init()
    {
        // 创建图
        graph = PlayableGraph.Create("Animation_Controller");
        // 设置图的时间模式
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        // 创建混合器
        mixer = AnimationMixerPlayable.Create(graph, 3);
        // 创建Output
        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(graph, "Animation", animator);
        // 让混合器链接上Output
        playableOutput.SetSourcePlayable(mixer);
    }

    public void DestoryNode(AnimationNodeBase node)
    {
        if (node!=null)
        {
            graph.Disconnect(mixer, node.InputPort);
            node.PushPool();
        }
    }

    private void StartTransitionAniamtion(float fixedTime)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(TransitionAniamtion(fixedTime));
    }

    // 动画过渡
    private IEnumerator TransitionAniamtion(float fixedTime)
    {
        // 交换端口号
        int temp = inputPort0;
        inputPort0 = inputPort1;
        inputPort1 = temp;

        // 硬切判断
        if (fixedTime == 0)
        {
            mixer.SetInputWeight(inputPort1, 0);
            mixer.SetInputWeight(inputPort0, 1);
        }

        // 当前的权重
        float currentWeight = 1;
        float speed = 1 / fixedTime;

        while (currentWeight > 0)
        {
            // 权重在减少
            currentWeight = Mathf.Clamp01(currentWeight - Time.deltaTime * speed);
            mixer.SetInputWeight(inputPort1, currentWeight);  // 减少
            mixer.SetInputWeight(inputPort0, 1 - currentWeight); // 增加
            yield return null;
        }
        transitionCoroutine = null;
    }

    /// <summary>
    /// 播放单个动画
    /// </summary>
    public void PlaySingleAniamtion(AnimationClip animationClip, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f)
    {
        SingleAnimationNode singleAnimationNode = null;
        if (currentNode == null) // 首次播放
        {
            singleAnimationNode = PoolManager.Instance.GetObject<SingleAnimationNode>();
            singleAnimationNode.Init(graph,mixer,animationClip,speed,inputPort0);
            mixer.SetInputWeight(inputPort0, 1);
        }
        else
        {
            SingleAnimationNode preNode = currentNode as SingleAnimationNode; // 上一个节点

            // 相同的动画
            if (!refreshAnimation && preNode != null && preNode.GetAnimationClip() == animationClip) return;
            // 销毁掉当前可能被占用的Node
            DestoryNode(previousNode);
            singleAnimationNode = PoolManager.Instance.GetObject<SingleAnimationNode>();
            singleAnimationNode.Init(graph, mixer, animationClip, speed, inputPort1);
            previousNode = currentNode;
            StartTransitionAniamtion(transitionFixedTime);
        }
        this.speed = speed;
        currentNode = singleAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }


    /// <summary>
    /// 播放混合动画
    /// </summary>
    public void PlayBlendAnimation(List<AnimationClip> clips,float speed = 1, float transitionFixedTime = 0.25f)
    {
        BlendAnimationNode blendAnimationNode = PoolManager.Instance.GetObject<BlendAnimationNode>();
        // 如果是第一次播放，不存在过渡
        if (currentNode == null)
        {
            blendAnimationNode.Init(graph, mixer, clips, speed, inputPort0);
            mixer.SetInputWeight(inputPort0, 1);
        }
        else
        {
            DestoryNode(previousNode);
            blendAnimationNode.Init(graph, mixer, clips, speed, inputPort1);
            previousNode = currentNode;
            StartTransitionAniamtion(transitionFixedTime);
        }
        this.speed = speed;
        currentNode = blendAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    /// <summary>
    /// 播放混合动画
    /// </summary>
    public void PlayBlendAnimation(AnimationClip clip1, AnimationClip clip2, float speed = 1, float transitionFixedTime = 0.25f)
    {
        BlendAnimationNode blendAnimationNode = PoolManager.Instance.GetObject<BlendAnimationNode>();
        // 如果是第一次播放，不存在过渡
        if (currentNode == null)
        {
            blendAnimationNode.Init(graph, mixer, clip1,clip2, speed, inputPort0);
            mixer.SetInputWeight(inputPort0, 1);
        }
        else
        {
            DestoryNode(previousNode);
            blendAnimationNode.Init(graph, mixer, clip1, clip2, speed, inputPort1);
            previousNode = currentNode;
            StartTransitionAniamtion(transitionFixedTime);
        }
        this.speed = speed;
        currentNode = blendAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    public void SetBlendWeight(List<float> weightList)
    {
        (currentNode as BlendAnimationNode).SetBlendWeight(weightList);
    }
    public void SetBlendWeight(float clip1Weight)
    {
        (currentNode as BlendAnimationNode).SetBlendWeight(clip1Weight);
    }


    private void OnDestroy()
    {
        graph.Destroy();
    }

    private void OnDisable()
    {
        graph.Stop();
    }

    #region RootMotion
    private Action<Vector3, Quaternion> rootMotionAction;
    private void OnAnimatorMove()
    {
        rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation);
    }
    public void SetRootMotionAction(Action<Vector3, Quaternion> rootMotionAction)
    {
        this.rootMotionAction = rootMotionAction;
    }
    public void ClearRootMotionAction()
    {
        rootMotionAction = null;
    }
    #endregion
    #region 动画事件
    private Dictionary<string, Action> eventDic = new Dictionary<string, Action>();
    // Animator会触发的实际事件函数
    private void AniamtionEvent(string eventName)
    {
        if (eventDic.TryGetValue(eventName,out Action action))
        {
            action?.Invoke();
        }
    }
    public void AddAniamtionEvent(string eventName,Action action)
    {
        if (eventDic.TryGetValue(eventName, out Action _action))
        {
            _action += action;
        }
        else
        {
            eventDic.Add(eventName,action);
        }
    }

    public void RemoveAnimationEvent(string eventName)
    {
        eventDic.Remove(eventName);
    }

    public void RemoveAnimationEvent(string eventName,Action action)
    {
        if (eventDic.TryGetValue(eventName,out Action _action))
        {
            _action -= action;
        }
    }

    public void CleanAllActionEvent()
    {
        eventDic.Clear();
    }
    #endregion

}
