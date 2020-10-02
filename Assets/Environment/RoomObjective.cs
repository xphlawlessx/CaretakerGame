﻿using System;
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
        private List<DestructableProp> _props;

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

        public int DestroyProp(DestructableProp prop)
        {
            _props.Remove(prop);

            Broadcast($"They're smashing up {name}");
            if (_props.Count < 1)
            {
                OnRoomDestroyed?.Invoke(this);
                Broadcast($"Room {name} is completley trashed");
            }

            return _props.Count;
        }
    }
}