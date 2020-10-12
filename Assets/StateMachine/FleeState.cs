using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class FleeState : EnemyState
    {
        private readonly LevelManager _lm;
        private readonly Transform _player;

        public FleeState(NavMeshAgent nav, Transform trans, Kid owner, Transform player, LevelManager lm) : base(nav,
            trans, owner)
        {
            Nav = nav;
            Transform = trans;
            Owner = owner;
            _player = player;
            _lm = lm;
        }

        public override void Init()
        {
            Nav.isStopped = false;
            Nav.SetDestination(_lm.GetExitPos());
            Decided = false;
            Nav.speed = Owner.Fsm.fleeSpeed;
        }

        public override void Run()
        {
            if (!Decided)
                if (Vector3.Distance(_player.position, Owner.transform.position) < Owner.runRadius)
                {
                    Decided = true;
                    if (!KeepFleeing()) Owner.Fsm.ChangeState(Owner.Fsm.ToProp);
                }

            if (Vector3.Distance(Transform.position, Nav.destination) < 2f)
            {
                Owner.Fsm.Disable.ResetT();
                Owner.Fsm.ChangeState(Owner.Fsm.Disable);
            }
        }

        private bool KeepFleeing()
        {
            return Random.Range(0, 10) == 0;
        }
    }
}