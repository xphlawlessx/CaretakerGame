using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class LeaveRoomState : EnemyState
    {
        private readonly LevelManager _lm;

        public LeaveRoomState(NavMeshAgent nav, Transform trans, Kid owner, LevelManager lm) : base(nav, trans, owner)
        {
            Nav = nav;
            Transform = trans;
            Owner = owner;
            _lm = lm;
        }

        public override void Init()
        {
            var room = _lm.GetRoom(Owner.Group);
            if (room == null)
            {
                Owner.Fsm.SetObjective(Owner.Group.Objective);
            }
            else
            {
                Owner.Fsm.SetObjective(room);
            }
            Owner.Fsm.ChangeState(Owner.Fsm.ToRoom);
        }

        public override void Run()
        {
            
            Owner.Fsm.ChangeState(Owner.Fsm.ToRoom);
        }
    }
}