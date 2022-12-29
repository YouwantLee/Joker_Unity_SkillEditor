using UnityEngine;
using System;
/// <summary>
/// 可序列化的颜色
/// </summary>
[Serializable]
public struct Serialization_Color
{
    public float r, g, b, a;
    public Serialization_Color(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
    public override string ToString()
    {
        return $"({r},{g},{b},{a})";
    }
    public override int GetHashCode()
    {
        return this.ConverToUnityColor().GetHashCode();
    }
}

public static class Serialization_ColorExtensions
{
    public static Color ConverToUnityColor(this Serialization_Color color)
    {
        return new Color(color.r, color.g, color.b, color.a);
    }

    public static Serialization_Color ConverToSerializationColor(this Color color)
    {
        return new Serialization_Color(color.r, color.g, color.b, color.a);
    }

}
