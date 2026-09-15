using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Utils
{
    private static readonly NormalItem.eNormalType[] m_normalTypes =
        (NormalItem.eNormalType[])Enum.GetValues(typeof(NormalItem.eNormalType));

    private static readonly List<NormalItem.eNormalType> m_allowedTypes =
        new List<NormalItem.eNormalType>(m_normalTypes.Length);

    public static NormalItem.eNormalType GetRandomNormalType()
    {
        return m_normalTypes[URandom.Range(0, m_normalTypes.Length)];
    }

    public static NormalItem.eNormalType GetRandomNormalTypeExcept(List<NormalItem.eNormalType> types)
    {
        if (types == null || types.Count == 0) return GetRandomNormalType();

        m_allowedTypes.Clear();
        for (int i = 0; i < m_normalTypes.Length; i++)
        {
            if (types.Contains(m_normalTypes[i])) continue;

            m_allowedTypes.Add(m_normalTypes[i]);
        }

        if (m_allowedTypes.Count == 0) return GetRandomNormalType();

        return m_allowedTypes[URandom.Range(0, m_allowedTypes.Count)];
    }
}
