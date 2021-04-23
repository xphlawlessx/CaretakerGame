using System;
using System.Collections.Generic;
using System.Linq;
using Environment;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Actors
{
    public class KidGroup
    {
        public Kid Leader
        {
            get
            {
                Kids.Sort(SortByLeadership);
                for (var i = 0; i < Kids.Count; i++)
                {
                    var kid = Kids[i];
                    kid.IsLeader = i == 0;
                }
                return Kids[0];
            }
        }

        public KidGroup()
        {
            var db = Enum.GetValues(typeof(DestructionBehaviour));
            DestructionBehaviour = (DestructionBehaviour) db.GetValue(Random.Range(0, db.Length));
            Kids = new List<Kid>();
        }

        public List<Kid> Kids { get; set; }

        public DestructionBehaviour DestructionBehaviour { get; set; }
        public RoomObjective Objective { get; set; }

        public Vector3 CenterPosition
        {
            get
            {
                var avg = new Vector3();
                avg = Kids.Aggregate(avg, (current, kid) => current + kid.transform.position);
                var result = avg / Kids.Count;
                result.y = 0;
                return result;
            }
        }
        
        private int SortByLeadership(Kid a, Kid b)
        {
            return a.Leadership.CompareTo(b.Leadership);
        }
    }
}