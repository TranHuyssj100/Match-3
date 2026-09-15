using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class Item
{
    public Cell Cell { get; private set; }

    public Transform View { get; private set; }

    private SpriteRenderer m_spriteRenderer;


    public virtual void SetView()
    {
        string prefabname = GetPrefabName();

        if (!string.IsNullOrEmpty(prefabname))
        {
            View = PoolManager.Spawn(prefabname);
            if (View)
            {
                m_spriteRenderer = View.GetComponent<SpriteRenderer>();

               
                if (m_spriteRenderer) m_spriteRenderer.sortingOrder = 0;

                ApplyView();
            }
        }
    }

    protected virtual string GetPrefabName() { return string.Empty; }

   
    protected virtual void ApplyView() { }

    protected void SetSprite(Sprite sprite)
    {
        if (m_spriteRenderer) m_spriteRenderer.sprite = sprite;
    }

    public virtual void SetCell(Cell cell)
    {
        Cell = cell;
    }

    internal void AnimationMoveToPosition()
    {
        if (View == null) return;

        View.DOMove(Cell.transform.position, 0.2f);
    }

    public void SetViewPosition(Vector3 pos)
    {
        if (View)
        {
            View.position = pos;
        }
    }

    public void SetViewRoot(Transform root)
    {
        if (View)
        {
            View.SetParent(root);
        }
    }

    public void SetSortingLayerHigher()
    {
        if (m_spriteRenderer)
        {
            m_spriteRenderer.sortingOrder = 1;
        }
    }


    public void SetSortingLayerLower()
    {
        if (m_spriteRenderer)
        {
            m_spriteRenderer.sortingOrder = 0;
        }
    }

    internal void ShowAppearAnimation()
    {
        if (View == null) return;

        Vector3 scale = View.localScale;
        View.localScale = Vector3.one * 0.1f;
        View.DOScale(scale, 0.1f);
    }

    internal virtual bool IsSameType(Item other)
    {
        return false;
    }

    internal virtual void ExplodeView()
    {
        if (View)
        {
            Transform view = View;

            ReleaseViewReference();

            view.DOScale(0.1f, 0.1f).OnComplete(() => PoolManager.Despawn(view));
        }
    }



    internal void AnimateForHint()
    {
        if (View)
        {
            View.DOPunchScale(View.localScale * 0.1f, 0.1f).SetLoops(-1);
        }
    }

    internal void StopAnimateForHint()
    {
        if (View)
        {
            View.DOKill();
        }
    }

    internal void Clear()
    {
        Cell = null;

        if (View)
        {
            PoolManager.Despawn(View);

            ReleaseViewReference();
        }
    }

    private void ReleaseViewReference()
    {
        View = null;
        m_spriteRenderer = null;
    }
}
