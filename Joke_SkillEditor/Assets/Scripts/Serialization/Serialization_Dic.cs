using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
/// <summary>
/// 可序列化的字典
/// </summary>
[Serializable]
public class Serialization_Dic<K,V>
{
    private List<K> keyList;
    private List<V> valueList;

    [NonSerialized] // 不用序列化
    private Dictionary<K,V> dictionary;
    public Dictionary<K, V> Dictionary { get => dictionary; }

    public Serialization_Dic()
    {
        this.dictionary = new Dictionary<K, V>();
    }
    public Serialization_Dic(Dictionary<K, V> dictionary)
    {
        this.dictionary = dictionary;
    }

    // 序列化的时候吧字典的内容放进list
    [OnSerializing] 
    private void OnSerializing(StreamingContext context)
    {
        keyList = new List<K>(dictionary.Count);
        valueList = new List<V>(dictionary.Count);
        foreach (var item in dictionary)
        {
            keyList.Add(item.Key);
            valueList.Add(item.Value);
        }
    }

    // 反序列化的时候，组装字典
    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        dictionary = new Dictionary<K, V>(keyList.Count);
        for (int i = 0; i < keyList.Count; i++)
        {
            dictionary.Add(keyList[i], valueList[i]);
        }
        keyList.Clear();
        valueList.Clear();
    }
}
