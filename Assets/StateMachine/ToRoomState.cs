using Environment;
using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class ToRoomState : EnemyState
    {
        private RoomObjective target;

        public ToRoomState(NavMeshAgent nav, Transform trans, Kid owner) : base(nav, trans, owner)
        {
            Nav = nav;
            Transform = trans;
            Owner = owner;
        }

        public override void Init()
        {
            target = Owner.TargetRoom;
            Nav.isStopped = false;
            if (target == null)
            {
                Owner.Fsm.ChangeState(Owner.Fsm.LeaveLevel);
                return;
            }


            Nav.SetDestination(target.transform.position);
        }

        public override void Run()
        {
            if (Owner.Fsm.IsInRoom) Owner.Fsm.ChangeState(Owner.Fsm.ToProp);
            if (target.GetProp() == null) Owner.Fsm.ChangeState(this);
            // var offset = new Vector3(0, 1, 0);
            // var path = Nav.path;
            // for (var i = 0; i < path.corners.Length - 1; i++)
            //     Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        }
    }
}