using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState
{
    private readonly float _TMax = 2.5f;
    protected float _T;
    protected bool Decided;
    protected NavMeshAgent Nav;
    protected Kid Owner;
    protected Transform Transform;

    protected EnemyState(NavMeshAgent nav, Transform trans, Kid owner)
    {
        Owner = owner;
        Nav = nav;
        Transform = trans;
    }

    public void ResetT()
    {
        _T = _TMax;
    }

    public abstract void Run();

    public virtual void Init()
    {
    }
}