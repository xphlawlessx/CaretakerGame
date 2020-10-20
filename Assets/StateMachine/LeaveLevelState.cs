using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class LeaveLevelState : EnemyState
    {
        private readonly LevelManager _lm;

        public LeaveLevelState(NavMeshAgent nav, Transform trans, Kid owner, LevelManager lm) : base(nav, trans,
            owner)
        {
            Nav = nav;
            Transform = trans;
            Owner = owner;
            _lm = lm;
        }

        public override void Init()
        {            Nav.isStopped = false;
            //Owner.Fsm.FleeTarget = ;
            Nav.SetDestination(_lm.GetExitPos());
        }

        public override void Run()
        {
            if (Vector3.Distance(Transform.position, Nav.destination) < 2f) Owner.Fsm.ChangeState(Owner.Fsm.Victory);
        }
    }
}