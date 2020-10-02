using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class DisabledState : EnemyState
    {
        public DisabledState(NavMeshAgent nav, Transform trans, Kid owner) : base(nav, trans, owner)
        {
            Nav = nav;
            Transform = trans;
            Owner = owner;
        }

        public override void Init()
        {
            Owner.Animate();
        }

        public override void Run()
        {
            Nav.SetDestination(Transform.position);
            _T -= Time.deltaTime;
            var disabled = _T > 0;
            if (!disabled)
            {
                Owner.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
                Owner.justSpawn = true;
                Owner.Fsm.ChangeState(Owner.Fsm.ToProp);
            }
        }
    }
}