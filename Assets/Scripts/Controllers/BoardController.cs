using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };

    public bool IsBusy { get; private set; }

    private Board m_board;

    private GameManager m_gameManager;

    private bool m_isDragging;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSettings m_gameSettings;

    private List<Cell> m_potentialMatch = new List<Cell>();

    private float m_timeAfterFill;

    private bool m_hintIsShown;

    private bool m_gameOver;

    private Vector3 m_lastDragPosition;

    private readonly List<Cell> m_matches = new List<Cell>();

    private readonly List<Cell> m_matchesHor = new List<Cell>();

    private readonly List<Cell> m_matchesVert = new List<Cell>();

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);

        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
        FindMatchesAndCollapse();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                StopHints();
                break;
        }
    }


    public void UpdateBoard()
    {
        if (m_gameOver) return;
        if (IsBusy) return;

        if (!m_hintIsShown)
        {
            m_timeAfterFill += Time.deltaTime;
            if (m_timeAfterFill > m_gameSettings.TimeForHint)
            {
                m_timeAfterFill = 0f;
                ShowHint();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_lastDragPosition = Input.mousePosition;

            Collider2D collider = GetColliderUnderPointer();
            if (collider != null)
            {
                m_isDragging = true;
                m_hitCollider = collider;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetRayCast();
        }

        if (Input.GetMouseButton(0) && m_isDragging)
        {
            //the physics query result can only change once the pointer has actually moved
            Vector3 pointer = Input.mousePosition;
            if ((pointer - m_lastDragPosition).sqrMagnitude < 1f) return;

            m_lastDragPosition = pointer;

            Collider2D collider = GetColliderUnderPointer();
            if (collider != null)
            {
                if (m_hitCollider != null && m_hitCollider != collider)
                {
                    StopHints();

                    Cell c1;
                    Cell c2;
                    if (m_hitCollider.TryGetComponent(out c1) && collider.TryGetComponent(out c2) && AreItemsNeighbor(c1, c2))
                    {
                        IsBusy = true;
                        SetSortingLayer(c1, c2);
                        m_board.Swap(c1, c2, () =>
                        {
                            FindMatchesAndCollapse(c1, c2);
                        });

                        ResetRayCast();
                    }
                }
            }
            else
            {
                ResetRayCast();
            }
        }
    }

    private Collider2D GetColliderUnderPointer()
    {
        return Physics2D.OverlapPoint(m_cam.ScreenToWorldPoint(Input.mousePosition));
    }

    private void ResetRayCast()
    {
        m_isDragging = false;
        m_hitCollider = null;
    }

    private void FindMatchesAndCollapse(Cell cell1, Cell cell2)
    {
        if (cell1.Item is BonusItem)
        {
            cell1.ExplodeItem();
            StartCoroutine(ShiftDownItemsCoroutine());
        }
        else if (cell2.Item is BonusItem)
        {
            cell2.ExplodeItem();
            StartCoroutine(ShiftDownItemsCoroutine());
        }
        else
        {
            m_matches.Clear();
            AppendMatches(cell1, m_matches);
            AppendMatches(cell2, m_matches);

            List<Cell> matches = m_matches;

            if (matches.Count < m_gameSettings.MatchesMin)
            {
                m_board.Swap(cell1, cell2, () =>
                {
                    IsBusy = false;
                });
            }
            else
            {
                OnMoveEvent();

                CollapseMatches(matches, cell2);
            }
        }
    }

    private void FindMatchesAndCollapse()
    {
        List<Cell> matches = m_board.FindFirstMatch();

        if (matches.Count > 0)
        {
            CollapseMatches(matches, null);
        }
        else
        {
            m_potentialMatch = m_board.GetPotentialMatches();
            if (m_potentialMatch.Count > 0)
            {
                IsBusy = false;

                m_timeAfterFill = 0f;
            }
            else
            {
                //StartCoroutine(RefillBoardCoroutine());
                StartCoroutine(ShuffleBoardCoroutine());
            }
        }
    }

    private void AppendMatches(Cell cell, List<Cell> result)
    {
        m_board.GetHorizontalMatches(cell, m_matchesHor);
        if (m_matchesHor.Count >= m_gameSettings.MatchesMin)
        {
            AppendDistinct(m_matchesHor, result);
        }

        m_board.GetVerticalMatches(cell, m_matchesVert);
        if (m_matchesVert.Count >= m_gameSettings.MatchesMin)
        {
            AppendDistinct(m_matchesVert, result);
        }
    }

    private static void AppendDistinct(List<Cell> source, List<Cell> result)
    {
        for (int i = 0; i < source.Count; i++)
        {
            if (result.Contains(source[i])) continue;

            result.Add(source[i]);
        }
    }

    private void CollapseMatches(List<Cell> matches, Cell cellEnd)
    {
        for (int i = 0; i < matches.Count; i++)
        {
            matches[i].ExplodeItem();
        }

        if(matches.Count > m_gameSettings.MatchesMin)
        {
            m_board.ConvertNormalToBonus(matches, cellEnd);
        }

        StartCoroutine(ShiftDownItemsCoroutine());
    }

    private IEnumerator ShiftDownItemsCoroutine()
    {
        m_board.ShiftDownItems();

        yield return new WaitForSeconds(0.2f);

        m_board.FillGapsWithNewItems();

        yield return new WaitForSeconds(0.2f);

        FindMatchesAndCollapse();
    }

    private IEnumerator RefillBoardCoroutine()
    {
        m_board.ExplodeAllItems();

        yield return new WaitForSeconds(0.2f);

        m_board.Fill();

        yield return new WaitForSeconds(0.2f);

        FindMatchesAndCollapse();
    }

    private IEnumerator ShuffleBoardCoroutine()
    {
        m_board.Shuffle();

        yield return new WaitForSeconds(0.3f);

        FindMatchesAndCollapse();
    }


    private void SetSortingLayer(Cell cell1, Cell cell2)
    {
        if (cell1.Item != null) cell1.Item.SetSortingLayerHigher();
        if (cell2.Item != null) cell2.Item.SetSortingLayerLower();
    }

    private bool AreItemsNeighbor(Cell cell1, Cell cell2)
    {
        return cell1.IsNeighbour(cell2);
    }

    internal void Clear()
    {
        m_board.Clear();
    }

    private void ShowHint()
    {
        m_hintIsShown = true;
        foreach (var cell in m_potentialMatch)
        {
            cell.AnimateItemForHint();
        }
    }

    private void StopHints()
    {
        m_hintIsShown = false;
        foreach (var cell in m_potentialMatch)
        {
            cell.StopHintAnimation();
        }

        m_potentialMatch.Clear();
    }
}
