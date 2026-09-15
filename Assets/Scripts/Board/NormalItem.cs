using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalItem : Item
{
    public enum eNormalType
    {
        TYPE_ONE,
        TYPE_TWO,
        TYPE_THREE,
        TYPE_FOUR,
        TYPE_FIVE,
        TYPE_SIX,
        TYPE_SEVEN
    }

    public eNormalType ItemType;

    public void SetType(eNormalType type)
    {
        ItemType = type;
    }

    protected override string GetPrefabName()
    {
        return Constants.PREFAB_NORMAL;
    }

    protected override void ApplyView()
    {
        ItemDataSO data = GameConfig.Instance != null ? GameConfig.Instance.ItemDataSO : null;
        if (data == null) return;

        SetSprite(data.GetSprite(ItemType));
    }

    internal override bool IsSameType(Item other)
    {
        NormalItem it = other as NormalItem;

        return it != null && it.ItemType == this.ItemType;
    }
}
