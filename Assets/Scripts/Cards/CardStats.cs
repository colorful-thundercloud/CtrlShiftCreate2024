using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardStats
{
    [SerializeField] List<Stat> stats;
    public CardStats(List<Stat> stats)
    {
        this.stats = stats;
    }
    public Stat GetStat(string name)
    {
        Stat result = null;
        foreach (var stat in stats)
            if (stat.Name == name) result = stat;
        return result;
    }
}
