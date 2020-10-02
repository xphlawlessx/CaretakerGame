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
        public KidGroup()
        {
            var db = Enum.GetValues(typeof(DestructionBehaviour));
            DestructionBehaviour = (DestructionBehaviour) db.GetValue(Random.Range(0, db.Length));
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

        public List<Kid> GetOtherKids(Kid self)
        {
            var query = from kid in Kids
                where kid != self
                select kid;
            return query.ToList();
        }
    }
}