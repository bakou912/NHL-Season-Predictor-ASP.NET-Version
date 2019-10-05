#region Header

// Author: Tommy Andrews
// File: PlayerComponents.cs
// Project: NHLPredictorASP
// Created: 09/30/2019

#endregion

using System.Collections.Generic;
using NHLPredictorASP.Classes.Entities;

namespace NHLPredictorASP.Classes.Deserialization
{
    #region Objects needed for deserialization of the JSON persons/stats coming from the NHL's API

    public class SeasonStats
    {
        public int Assists { get; set; }
        public int Goals { get; set; }
        public int Games { get; set; }
    }

    public class League
    {
        public int Id { get; set; }
    }

    public class Split
    {
        public SeasonStats Stat { get; set; }
        public League League { get; set; }

        public string Season { get; set; }
    }

    public class Stat
    {
        public List<Split> Splits { get; set; }
    }

    public class StatsList
    {
        public List<Stat> Stats { get; set; }
    }

    public class Position
    {
        public string Code { get; set; }
    } 

    public class Person
    {
        public Person()
        {
            Active = true;
        }

        public string Id { get; set; }
        public string FullName { get; set; }
        public string TeamAbv { get; set; }
        public bool Active { get; set; }
    }

    #endregion Objects needed for deserialization of the JSON persons/stats coming from the NHL's API
}