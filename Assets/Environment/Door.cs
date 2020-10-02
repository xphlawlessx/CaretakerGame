using DefaultNamespace;
using Environment;
using UnityEngine;

public class Door : UIBroadcaster
{
    [SerializeField] private GameObject mesh;
    private bool _isOpen;
    private RoomObjective _room;

    private void Start()
    {
        _room = GetComponentInParent<RoomObjective>();
        Setup();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isOpen)
        {
            if (other.CompareTag("Enemy"))
            {
                var enemy = other.GetComponent<Kid>();
                var fsm = enemy.Fsm;
                if (fsm.State != fsm.Flee && fsm.State != fsm.LeaveLevel && fsm.State != fsm.LeaveRoom)
                {
                    Broadcast($"Kid spotted entering {_room.name}");
                    enemy.Fsm.ChangeState(fsm.ToProp);
                }
            }
            else if (other.CompareTag("Player"))
            {
                _room.InvokeOnPlayerEnter();
            }

            mesh.SetActive(false);
            _isOpen = true;
        }
        else
        {
            _isOpen = false;
            mesh.SetActive(true);
        }
    }
}