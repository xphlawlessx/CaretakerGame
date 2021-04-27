using Environment;
using UnityEngine;
using UserInterface;

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
            var fsm = other.GetComponent<Kid>().Fsm;
            fsm.IsInRoom = !fsm.IsInRoom;
            if (fsm.IsInRoom)
                if (fsm.State == fsm.ToRoom)
                    Broadcast($"Kid spotted entering {_room.name} in the {_room.area} ");

            if (fsm.State != fsm.Flee && fsm.State != fsm.LeaveLevel) fsm.EnterOrLeaveRoom(_room);
        }
        else if (other.CompareTag("Player"))
        {
            _room.InvokeOnPlayerEnter();
        }

        _isOpen = !_isOpen;
        mesh.SetActive(_isOpen);
    }
}