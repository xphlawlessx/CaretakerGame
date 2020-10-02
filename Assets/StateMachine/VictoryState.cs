using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class VictoryState : EnemyState
    {
        private readonly LevelManager _lm;

        public VictoryState(NavMeshAgent nav, Transform trans, Kid owner, LevelManager lm) : base(nav, trans, owner)
        {
            Nav = nav;
            Transform = trans;
            Owner = owner;
            _lm = lm;
        }

        public override void Init()
        {
            _lm.DestroyKid(Owner);
        }

        public override void Run()
        {
        }
    }
}