using UnityEngine;

public class LevelEnterExit : MonoBehaviour
{
    private LevelManager _lm;

    private void Start()
    {
        _lm = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        var e = other.GetComponent<Kid>();
        if (e.justSpawn) return;
        // var state = e.Fsm?.State;
        // if (state == null || state == e.Fsm.Disable || state == e.Fsm.Victory) return;
        // if (e.Fsm.State == e.Fsm.LeaveLevel) e.Fsm.ChangeState(e.Fsm.Victory);
        e.DisableMe();
    }
}