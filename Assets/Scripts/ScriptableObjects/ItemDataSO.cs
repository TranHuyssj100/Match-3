using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "ScriptableObjects/ItemDataSO")]
[System.Serializable]

public class ItemDataSO : ScriptableObject
{
    public List<ItemData> Items;

    public ItemData GetItemData(NormalItem.eNormalType type)
    {
        return Items.Find(item => item.name == type);
    }
    public Sprite GetSprite(NormalItem.eNormalType type)
    {
        ItemData data = GetItemData(type);
        return data != null ? data.sprite : null;
    }
}

[System.Serializable]
public class ItemData
{
    public NormalItem.eNormalType name;
    public Sprite sprite;
}