using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Stat
    [Serializable]
    public class Stat
    {
        public int level;
        public int maxHp;
        public int attack;
        public int totalExp;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();

            foreach (Stat stat in stats)
                dict.Add(stat.level, stat);

            return dict;
        }
    }

    #endregion

    [Serializable]
    public class MonData : ILoader<string, Stat>
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<string, Stat> MakeDict()
        {
            Dictionary<string, Stat> dict = new Dictionary<string, Stat>();

            foreach (Stat stat in stats)
                dict.Add(stat.level.ToString(), stat);

            return dict;
        }
    }
}

