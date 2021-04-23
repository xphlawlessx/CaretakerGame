using System;
using Environment;
using UnityEngine;

public class DestructableProp : MonoBehaviour
{
    [SerializeField] private GameObject _brokenVersion;
    [SerializeField] private int Value = 500;
    public RoomObjective room;
    private int hp = 1;

    private void Start()
    {
        room = GetComponentInParent<RoomObjective>();
        var lm = FindObjectOfType<LevelManager>();
        OnDestroyed += lm.AddDamageCashValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        // var fsm = other.GetComponent<Kid>().Fsm;
        // if (fsm.State != fsm.Flee && fsm.State != fsm.LeaveLevel)
        // {
        //     var enemy = other.GetComponent<Kid>();
        //     //enemy.Fsm.TargetProp = this;
        //     //enemy.Fsm.ChangeState(fsm.DestroyTarget);
        // }
    }

    private event Action<int> OnDestroyed;

    public int GetHit(Kid by)
    {
        hp -= 1;
        if (hp <= 0)
        {
            var prop = Instantiate(_brokenVersion, transform.position, transform.rotation);
            var broken = prop.GetComponent<BrokenProp>();
            if (broken != null)
            {
                broken.brokenBy = by;
                broken.Value = Value;
            }

            OnDestroyed?.Invoke(Value);
            Destroy(gameObject);
            if (room == null) return 0;
            var num = room.DestroyProp(this);
            return num;
        }

        return -1;
    }
}