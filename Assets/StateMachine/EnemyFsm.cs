using Actors;
using Environment;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace StateMachine
{
    public class EnemyFsm
    {
        private readonly LevelManager _lm;
        private readonly NavMeshAgent _nav;
        private readonly Kid _owner;

        public readonly EnemyState Disable;
        public readonly EnemyState Flee;
        public readonly EnemyState LeaveLevel;
        public readonly EnemyState LeaveRoom;
        public readonly EnemyState ToProp;
        public readonly EnemyState ToRoom;
        public readonly EnemyState Victory;
        public float baseSpeed = 7f;
        public EnemyState DestroyTarget;

        public float fleeSpeed = 10f;

        public bool IsInRoom;

        public EnemyState State;
        public DestructableProp TargetProp;

        public EnemyFsm(NavMeshAgent nav, Transform transform, Kid owner, Transform player, LevelManager lm)
        {
            _nav = nav;
            if (nav.speed != baseSpeed) baseSpeed = nav.speed;
            _owner = owner;
            _lm = lm;
            DestroyTarget = new DestroyTargetState(_nav, transform, _owner);
            Disable = new DisabledState(_nav, transform, _owner);
            Flee = new FleeState(_nav, transform, _owner, player, _lm);
            LeaveLevel = new LeaveLevelState(_nav, transform, _owner, _lm);
            LeaveRoom = new LeaveRoomState(_nav, transform, _owner, _lm);
            ToProp = new ToPropState(_nav, transform, _owner);
            Victory = new VictoryState(_nav, transform, _owner, _lm);
            ToRoom = new ToRoomState(_nav, transform, _owner);

            owner.Fsm = this;
            SetObjective(_lm.GetRoom(_owner.Group));
            ChangeState(ToRoom);
        }


        public void EnterOrLeaveRoom(RoomObjective room)
        {
            IsInRoom = !IsInRoom;
            if (IsInRoom)
            {
                room.SetLights(true);
                room.OnPlayerEntered -= _owner.RunAway;
                room.OnRoomDestroyed -= RoomIsDestroyed;
            }

            room.OnPlayerEntered += _owner.RunAway;
            room.OnRoomDestroyed += RoomIsDestroyed;
        }

        public void ChangeState(EnemyState to)
        {
            //Debug.Log("from - " + State + " to -" + to);
            if (State == Flee) _nav.speed = baseSpeed;
            to.Init();
            State = to;
            _owner.AnimateOrNot();
            _owner.GetComponentInChildren<FaceCamera>().GetComponentInChildren<TextMeshPro>().text =
                State.ToString().Split('.')[1].Replace("State", "");
        }

        public void SetObjective(RoomObjective room)
        {
            var leader = _owner.Group.Leader;
            if (_owner.IsLeader)
            {
                var hits = Physics.OverlapSphere(_owner.transform.position, 25f, LayerMask.GetMask("Enemy"));
                foreach (var hit in hits)
                {
                    var kid = hit.GetComponent<Kid>();
                    if (leader.Leadership < _owner.Leadership) kid.ChangeGroup(_owner.Group);
                }
            }
            else if (_owner.Lonewolf > _owner.Group.Leader.Leadership)
            {
                _owner.ChangeGroup(new KidGroup());
            }

            _owner.Group.Objective = room;
        }

        [CanBeNull]
        public DestructableProp GetDestructionTarget()
        {
            var objective = _owner.Group.Objective;
            if (objective == null) ChangeState(LeaveLevel);
            return objective.GetProp();
        }

        public void Run()
        {
            //Debug.Log(_owner.name + " - " + State);
            State?.Run();
        }

        private void RoomIsDestroyed(RoomObjective room)
        {
            ChangeState(LeaveRoom);
        }
    }
}