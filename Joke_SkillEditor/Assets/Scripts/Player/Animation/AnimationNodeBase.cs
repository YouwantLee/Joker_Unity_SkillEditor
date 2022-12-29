using JKFrame;
/// <summary>
/// 动画节点基类
/// </summary>
public abstract class AnimationNodeBase
{
    public int InputPort;

    public abstract void SetSpeed(float speed);
    public virtual void PushPool()
    {
        this.JKObjectPushPool();
    }
}