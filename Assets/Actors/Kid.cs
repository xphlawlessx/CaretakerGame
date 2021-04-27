﻿using System;
using Environment;
using StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UserInterface;
using Random = UnityEngine.Random;

public class Kid : UIBroadcaster
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");
    private static readonly int Destroy1 = Animator.StringToHash("Destroy");
    private readonly float _notorietyMax = 10f;
    private readonly float _spawnTMax = 1f;
    private readonly float trackTMax = 30f;

    private Animator _anim;
    private bool _isInLight;
    private bool _isLosingNotoriety;
    private bool _isRunning;
    private NavMeshAgent _nav;
    private float _notoriety;
    private Transform _player;
    private float _spawnT = 1f;

    private LineRenderer footPrints;

    public EnemyFsm Fsm;
    public bool isIdentified;
    private bool isTracked;
    private bool isVisibleOnMap;
    public bool justSpawn = true;
    public int Leadership; //TODO : random?
    public GameObject minimapIndicator;
    private float miniMapT;
    public bool playerHasGoodMemory;
    public float runRadius = 10f;
    private float trackT = 30f;
    public RoomObjective TargetRoom { get; set; }
    public DestructableProp TargetProp { get; set; }

    public bool IsInLight
    {
        get => _isInLight;
        set
        {
            _isLosingNotoriety = !value;
            _isInLight = value;
            if (value)
                RunAway();
        }
    }


    private void Start()
    {
        _spawnT = _spawnTMax;
        _player = FindObjectOfType<FirstPersonController>().transform;
        _nav = GetComponent<NavMeshAgent>();
        var lm = FindObjectOfType<LevelManager>();
        Fsm = new EnemyFsm(_nav, transform, this, _player, lm);
        _anim = GetComponent<Animator>();
        justSpawn = true;
        Setup();
        footPrints = GetComponentInChildren<LineRenderer>();
        Leadership = Random.Range(1, 6);
    }

    private void Update()
    {
        var delta = Time.deltaTime;
        if (isTracked) TrackMe(delta);

        if (isVisibleOnMap) ShowOnMap(delta);

        if (Fsm == null) return;
        Fsm.Run();
        if (IsInLight)
        {
            if (Vector3.Distance(_player.position, transform.position) > 15f)
            {
                IsInLight = false;
                return;
            }

            _notoriety += delta;
            if (_notoriety >= _notorietyMax)
            {
                Broadcast("I knew that was you Jimmy!");
                isIdentified = true;
                Fsm.ChangeState(Fsm.LeaveLevel);
            }
        }
        else if (_isLosingNotoriety && _notoriety > 0)
        {
            _notoriety -= playerHasGoodMemory ? delta * 0.5f : delta;
        }

        _notoriety = Mathf.Clamp(_notoriety, 0, _notorietyMax);
    

        if (justSpawn)
        {
            justSpawn = _spawnT > 0;
            _spawnT -= delta;
        }
    }

    private void ShowOnMap(float delta)
    {
        miniMapT -= delta;
        if (miniMapT <= 0)
        {
            minimapIndicator.SetActive(false);
            isVisibleOnMap = false;
        }
    }

    private void TrackMe(float delta)
    {
        trackT -= delta;
        if (trackT <= 0)
        {
            isTracked = false;
            footPrints.positionCount = 0;
        }
    }

    public void ShowOnMiniMap(float time)
    {
        miniMapT = time;
        isVisibleOnMap = true;
        minimapIndicator.SetActive(true);
    }
    

    public void AnimateOrNot()
    {
        if (_anim == null) _anim = GetComponent<Animator>();
        var state = Fsm.State;
        if (state == Fsm.DestroyTarget)
        {
            _anim.SetTrigger(Destroy1);
        }
        else
        {
            var isWalking = state != Fsm.Disable && state != Fsm.DestroyTarget;
            _anim.SetBool(IsWalk, isWalking);
        }
    }

    public void Track()
    {
        isTracked = true;
        trackT = trackTMax;
        var path = new NavMeshPath();
        _nav.CalculatePath(_player.transform.position, path);
        footPrints.positionCount = path.corners.Length;
        footPrints.SetPositions(path.corners);
    }

    public void DisableMe()
    {
        Fsm.Disable.ResetT();
        Fsm.ChangeState(Fsm.Disable);
    }


    public void RunAway()
    {
        if (Fsm.State == Fsm.Flee || Fsm.State == Fsm.LeaveLevel) return;
        Fsm.ChangeState(Fsm.Flee);
    }

    public void LeaveLevel()
    {
        Fsm?.ChangeState(Fsm.LeaveLevel);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var kid = other.GetComponent<Kid>();
            if (kid.Leadership >= Leadership) return;
            kid.Fsm.SetObjective(TargetRoom);
        }
    }
}