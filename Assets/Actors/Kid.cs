﻿using System;
using Actors;
using DefaultNamespace;
using StateMachine;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Kid : UIBroadcaster, IKidClass
{
    private static readonly int IsWalk = Animator.StringToHash("IsWalk");
    private static readonly int Destroy1 = Animator.StringToHash("Destroy");
    public float runRadius = 10f;
    public bool justSpawn = true;
    public bool isIdentified;
    public bool playerHasGoodMemory;
    public GameObject minimapIndicator;
    private readonly float _ambientDamageTMax = 1f;
    private readonly float _hpMax = 10f;
    private readonly float _spawnTMax = 1f;
    private readonly float trackTMax = 30f;

    private float _ambientDamageT;
    private Animator _anim;
    private float _hp;
    private bool _isInLight;
    private bool _isRegen;
    private bool _isRunning;
    private NavMeshAgent _nav;
    private Transform _player;
    private float _spawnT = 1f;

    private LineRenderer footPrints;

    public EnemyFsm Fsm;
    public KidGroup Group;
    private bool isTracked;
    private bool isVisibleOnMap;
    private float miniMapT;
    private float trackT = 30f;

    public bool IsInLight
    {
        get => _isInLight;
        set
        {
            _isRegen = !value;
            _isInLight = value;
            if (value)
                RunAway();
        }
    }

    private void Start()
    {
        _hp = _hpMax;
        _spawnT = _spawnTMax;
        _player = FindObjectOfType<FirstPersonController>().transform;
        _nav = GetComponent<NavMeshAgent>();
        var lm = FindObjectOfType<LevelManager>();
        //Fsm = new EnemyFsm(_nav, transform, this, _player, lm);
        OnAmbientDamage += lm.AddDamageCashValue;
        _ambientDamageT = _ambientDamageTMax;
        _anim = GetComponent<Animator>();
        justSpawn = true;
        AmbientDamage = 1; //Todo TempValue Do Something better
        SetUpClass();
        Setup();
        footPrints = GetComponentInChildren<LineRenderer>();
    }

    private void Update()
    {
        var delta = Time.deltaTime;
        if (isTracked) TrackMe(delta);

        if (isVisibleOnMap) ShowOnMap(delta);

        //if (Fsm == null) return;
        //Fsm.Run();
        if (IsInLight)
        {
            if (Vector3.Distance(_player.position, transform.position) > 15f)
            {
                IsInLight = false;
                return;
            }

            _hp -= delta;
            if (_hp <= 0)
            {
                Broadcast("I knew that was you Jimmy!");
                isIdentified = true;
                // Fsm.ChangeState(Fsm.Victory);
            }
        }
        else if (_isRegen && _hp < _hpMax)
        {
            _hp += playerHasGoodMemory ? delta * 0.5f : delta;
        }

        _hp = Mathf.Clamp(_hp, 0, _hpMax);
        if (_hp == _hpMax)
        {
            _ambientDamageT -= delta;
            if (_ambientDamageT <= 0)
            {
                _ambientDamageT = _ambientDamageTMax;
                OnAmbientDamage?.Invoke(AmbientDamage);
            }
        }

        if (justSpawn)
        {
            justSpawn = _spawnT > 0;
            _spawnT -= delta;
        }
    }

    public bool IsLeader { get; set; }
    public int Leadership { get; set; }
    public int Lonewolf { get; set; }

    public Clothing Clothing { get; set; }
    public int AmbientDamage { get; set; }

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

    private event Action<int> OnAmbientDamage;

    private void SetUpClass()
    {
        Leadership = Random.Range(1, 7);
        Lonewolf = Random.Range(1, 11);
        var cl = Enum.GetValues(typeof(Clothing));
        Clothing = (Clothing) cl.GetValue(Random.Range(0, cl.Length));
    }

    public void ChangeGroup(KidGroup to)
    {
        Group.Kids.Remove(this);
        to.Kids.Add(this);
        Group = to;
        
        
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
        Debug.Log("track");
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
}