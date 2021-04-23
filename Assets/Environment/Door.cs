using DefaultNamespace;
using Environment;
using UnityEngine;

public class Door : UIBroadcaster
{
    private bool _isOpen;
    private RoomObjective _room;
    [SerializeField] private GameObject mesh;

    private void Start()
    {
        _room = GetComponentInParent<RoomObjective>();
        Setup();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Kid>();
            var fsm = enemy.Fsm;
            if (!fsm.IsInRoom)
            {
                // if (fsm.State != fsm.Flee && fsm.State != fsm.LeaveLevel && fsm.State != fsm.LeaveRoom)
                //{
                //}

                Broadcast($"Kid spotted entering {_room.name} in the {_room.area} ");
                enemy.Fsm.IsInRoom = true;
            }
            else
            {
                enemy.Fsm.IsInRoom = false;
            }

            if (fsm.State != fsm.Flee && fsm.State != fsm.LeaveLevel) enemy.Fsm.EnterOrLeaveRoom(_room);
        }
        else if (other.CompareTag("Player"))
        {
            _room.InvokeOnPlayerEnter();
        }

        _isOpen = !_isOpen;
        mesh.SetActive(_isOpen);
    }
}