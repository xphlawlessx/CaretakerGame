using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class ToPropState : EnemyState
    {
        private DestructableProp target;

        public ToPropState(NavMeshAgent nav, Transform trans, Kid owner) : base(nav, trans, owner)
        {
            Nav = nav;
            Transform = trans;
            Owner = owner;
        }

        public override void Init()
        {
            target = Owner.Fsm.GetDestructionTarget();
            Owner.TargetProp = target;
            Nav.isStopped = false;
            if (target == null)
            {
                Owner.Fsm.ChangeState(Owner.Fsm.LeaveRoom);
                return;
            }


            Nav.SetDestination(target.transform.position);
        }

        public override void Run()
        {
            var pos = Owner.transform.position;
            var nearByRbProps = Physics.OverlapSphere(pos, 2f, LayerMask.GetMask("rbProps"));
            if (nearByRbProps.Length > 0)
            {
                if (Random.Range(0, 5) == 0) return;
                var prop = nearByRbProps[Random.Range(0, nearByRbProps.Length)];
                var propPos = prop.transform.position;
                var dir = (propPos - pos).normalized;
                var rb = prop.GetComponent<Rigidbody>();
                rb.AddExplosionForce(350f, propPos - dir, 2f);
            }

            if (Nav.remainingDistance < 1) Owner.Fsm.ChangeState(Owner.Fsm.DestroyTarget);

            // var offset = new Vector3(0, 1, 0);
            // var path = Nav.path;
            // for (var i = 0; i < path.corners.Length - 1; i++)
            //     Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            //if (target == null) Owner.Fsm.ChangeState(this);
        }
    }
}