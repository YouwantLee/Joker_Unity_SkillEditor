using UnityEngine;
using System;
/// <summary>
/// 可序列化的Vector3
/// </summary>
[Serializable]
public class Serialization_Vector3
{
    public float x, y, z;

    public Serialization_Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public override string ToString()
    {
        return $"({x},{y},{z})";
    }
    public override int GetHashCode()
    {
        return this.ConverToUnityVector3().GetHashCode();
    }
}

/// <summary>
/// 可序列化的Vector2
/// </summary>
[Serializable]
public class Serialization_Vector2
{
    public float x, y;

    public Serialization_Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    public override string ToString()
    {
        return $"({x},{y})";
    }
    public override int GetHashCode()
    {
        return this.ConverToUnityVector2().GetHashCode();
    }
}


public static class SerializationVectorExtensions
{
    public static Vector3 ConverToUnityVector3(this Serialization_Vector3 sV3)
    {
        return new Vector3(sV3.x, sV3.y, sV3.z);
    }
    public static Serialization_Vector3 ConverToUnityVector3(this Vector3 sV3)
    {
        return new Serialization_Vector3(sV3.x, sV3.y, sV3.z);
    }
    public static Vector3Int ConverToUnityVector3Int(this Serialization_Vector3 sV3)
    {
        return new Vector3Int((int)sV3.x, (int)sV3.y, (int)sV3.z);
    }
    public static Serialization_Vector3 ConverToUnityVector3(this Vector3Int sV3)
    {
        return new Serialization_Vector3(sV3.x, sV3.y, sV3.z);
    }

    public static Vector2 ConverToUnityVector2(this Serialization_Vector2 sV2)
    {
        return new Vector2(sV2.x, sV2.y);
    }
    public static Serialization_Vector2 ConverToUnityVector2(this Vector2 sV2)
    {
        return new Serialization_Vector2(sV2.x, sV2.y);
    }
    public static Vector3Int ConverToUnityVector2Int(this Serialization_Vector2 sV2)
    {
        return new Vector3Int((int)sV2.x, (int)sV2.y);
    }
    public static Serialization_Vector2 ConverToUnityVector2(this Vector2Int sV2)
    {
        return new Serialization_Vector2(sV2.x, sV2.y);
    }

}
