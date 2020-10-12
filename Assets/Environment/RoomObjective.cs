using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    public class RoomObjective : UIBroadcaster
    {
        public int numProps;

        private Door[] _doors;

        private bool _isDoorOpen;

        private GameObject _light;
        private List<DestructableProp> _props;
        public LevelArea area;
        public Vector3 DoorPos
        {
            get
            {
                var pos = _doors[0].transform.position;
                pos.y = 0;
                return pos;
            }
        }

        private void Start()
        {
            _doors = GetComponentsInChildren<Door>();
            _light = GetComponentInChildren<Light>().gameObject;
            _light.SetActive(false);
            _props = GetComponentsInChildren<DestructableProp>().ToList();
            numProps = _props.Count;
            Setup();
        }

        public event Action OnPlayerEntered;

        public event Action<RoomObjective> OnRoomDestroyed;

        public DestructableProp GetProp()
        {
            if (_props.Count == 0)
            {
                OnRoomDestroyed?.Invoke(this);
                return null;
            }

            return _props[Random.Range(0, _props.Count)];
        }

        public void InvokeOnPlayerEnter()
        {
            OnPlayerEntered?.Invoke();
        }

        public void SetLights(bool on)
        {
            _light.SetActive(on);
        }

        public int DestroyProp(DestructableProp prop)
        {
            _props.Remove(prop);

            Broadcast($"They're smashing up {name} , get to the {area} ");
            if (_props.Count < 1)
            {
                OnRoomDestroyed?.Invoke(this);
                Broadcast($"Room {name} in {area} is completely trashed");
            }

            return _props.Count;
        }
    }

    public enum LevelArea
    {
        NorthWestWing , NorthEastWing, SouthWestWing, SouthEastWing
    }
}