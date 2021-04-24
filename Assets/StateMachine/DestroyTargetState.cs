using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class DestroyTargetState : EnemyState
    {
        private DestructableProp _target;

        public DestroyTargetState(NavMeshAgent nav, Transform trans, Kid owner) : base(nav, trans, owner)
        {
            Nav = nav;
            Transform = trans;
            Owner = owner;
        }

        public override void Init()
        {
            Nav.isStopped = true;
            _target = Owner.TargetProp;
        }

        public override void Run()
        {
            _T -= Time.deltaTime;
            if (_T <= 0)
            {
                if (_target == null)
                {
                    Owner.Fsm.ChangeState(Owner.Fsm.LeaveRoom);
                    return;
                }

                var hit = _target.GetHit(Owner);
                if (hit > 0)
                    Owner.Fsm.ChangeState(Owner.Fsm.ToProp);
                else if (hit == 0) Owner.Fsm.ChangeState(Owner.Fsm.LeaveRoom);
                ResetT();
            }
        }
    }
}