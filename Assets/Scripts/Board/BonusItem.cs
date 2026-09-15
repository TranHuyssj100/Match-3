using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusItem : Item
{
    public enum eBonusType
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    public eBonusType ItemType;

    private static readonly List<Cell> s_explodeBuffer = new List<Cell>(16);

    public void SetType(eBonusType type)
    {
        ItemType = type;
    }

    protected override string GetPrefabName()
    {
        string prefabname = string.Empty;
        switch (ItemType)
        {
            case eBonusType.NONE:
                break;
            case eBonusType.HORIZONTAL:
                prefabname = Constants.PREFAB_BONUS_HORIZONTAL;
                break;
            case eBonusType.VERTICAL:
                prefabname = Constants.PREFAB_BONUS_VERTICAL;
                break;
            case eBonusType.ALL:
                prefabname = Constants.PREFAB_BONUS_BOMB;
                break;
        }

        return prefabname;
    }

    internal override bool IsSameType(Item other)
    {
        BonusItem it = other as BonusItem;

        return it != null && it.ItemType == this.ItemType;
    }

    internal override void ExplodeView()
    {
        ActivateBonus();

        base.ExplodeView();
    }

    private void ActivateBonus()
    {
        switch (ItemType)
        {
            case eBonusType.HORIZONTAL:
                ExplodeHorizontalLine();
                break;
            case eBonusType.VERTICAL:
                ExplodeVerticalLine();
                break;
            case eBonusType.ALL:
                ExplodeBomb();
                break;
        }
    }

    private void ExplodeBomb()
    {
        s_explodeBuffer.Clear();

        if (Cell.NeighbourBottom) s_explodeBuffer.Add(Cell.NeighbourBottom);
        if (Cell.NeighbourUp) s_explodeBuffer.Add(Cell.NeighbourUp);

        if (Cell.NeighbourLeft)
        {
            s_explodeBuffer.Add(Cell.NeighbourLeft);
            if (Cell.NeighbourLeft.NeighbourUp)    s_explodeBuffer.Add(Cell.NeighbourLeft.NeighbourUp);
            if (Cell.NeighbourLeft.NeighbourBottom) s_explodeBuffer.Add(Cell.NeighbourLeft.NeighbourBottom);
        }

        if (Cell.NeighbourRight)
        {
            s_explodeBuffer.Add(Cell.NeighbourRight);
            if (Cell.NeighbourRight.NeighbourUp)    s_explodeBuffer.Add(Cell.NeighbourRight.NeighbourUp);
            if (Cell.NeighbourRight.NeighbourBottom) s_explodeBuffer.Add(Cell.NeighbourRight.NeighbourBottom);
        }

        for (int i = 0; i < s_explodeBuffer.Count; i++)
        {
            s_explodeBuffer[i].ExplodeItem();
        }
    }

    private void ExplodeVerticalLine()
    {
        s_explodeBuffer.Clear();

        Cell newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourUp;
            if (next == null) break;

            s_explodeBuffer.Add(next);
            newcell = next;
        }

        newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourBottom;
            if (next == null) break;

            s_explodeBuffer.Add(next);
            newcell = next;
        }

        for (int i = 0; i < s_explodeBuffer.Count; i++)
        {
            s_explodeBuffer[i].ExplodeItem();
        }
    }

    private void ExplodeHorizontalLine()
    {
        s_explodeBuffer.Clear();

        Cell newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourRight;
            if (next == null) break;

            s_explodeBuffer.Add(next);
            newcell = next;
        }

        newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourLeft;
            if (next == null) break;

            s_explodeBuffer.Add(next);
            newcell = next;
        }

        for (int i = 0; i < s_explodeBuffer.Count; i++)
        {
            s_explodeBuffer[i].ExplodeItem();
        }
    }
}
