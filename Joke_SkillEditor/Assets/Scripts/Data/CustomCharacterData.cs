using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 自定角色的全部数据
/// </summary>
[Serializable]
public class CustomCharacterData
{
    public Serialization_Dic<int, CustomCharacterPartData> CustomPartDataDic;
}

/// <summary>
/// 自定义角色部位的数据
/// </summary>
[Serializable]
public class CustomCharacterPartData
{
    public int Index;
    public float Size;
    public float Height;
    public Serialization_Color Color1;
    public Serialization_Color Color2;
}

