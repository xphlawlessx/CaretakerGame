using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject minimapIndicator;
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
    private Shader _trackShader;

    private List<Shader> baseShaders = new List<Shader>();
    public EnemyFsm Fsm;
    public KidGroup Group;
    private bool isTracked;
    private bool isVisibleOnMap;
    private float miniMapT;
    private readonly Vector3 offset = new Vector3(0, 0.5f, 0);
    private SkinnedMeshRenderer[] rends;
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
        
        _trackShader = Shader.Find("Custom/Tracked");
        _player = FindObjectOfType<FirstPersonController>().transform;
        _nav = GetComponent<NavMeshAgent>();
        var lm = FindObjectOfType<LevelManager>();
        Fsm = new EnemyFsm(_nav, transform, this, _player,lm);
        OnAmbientDamage += lm.AddDamageCashValue;
        _ambientDamageT = _ambientDamageTMax;
        _anim = GetComponent<Animator>();
        justSpawn = true;
        AmbientDamage = 1; //Todo TempValue Do Something better
        SetUpClass();
        Setup();

        baseShaders = new List<Shader>();
        rends = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var rend in rends)
        foreach (var mat in rend.materials)
            baseShaders.Add(mat.shader);
    }

    private void Update()
    {
        var delta = Time.deltaTime;
        if (isTracked)
        {
            TrackMe(delta);
        }

        if (isVisibleOnMap)
        {
            ShowOnMap(delta);
        }

        if (Fsm == null) return;
        Fsm.Run();
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
                BroadcastMessage("I knew that was you Jimmy!");
                isIdentified = true;
                Fsm.ChangeState(Fsm.Victory);
            }
        }
        else if (_isRegen && _hp < _hpMax)
        {
            _hp += delta;
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

    private void ShowOnMap(float delta)
    {
        miniMapT -= delta;
        if (miniMapT <= 0)
        {
            minimapIndicator.SetActive(false);
            isVisibleOnMap = false;
        }
    }

    public GroupBehaviour GroupBehaviour { get; set; }

    public Clothing Clothing { get; set; }
    public int AmbientDamage { get; set; }

    private void TrackMe(float delta)
    {
        trackT -= delta;
        var ray = new Ray(transform.position + offset,
            (_player.position + offset - transform.position + offset).normalized);
        if (Physics.SphereCast(ray, 1.5f, out var hit,
            Vector3.Distance(_player.position, transform.position),
            LayerMask.GetMask("Wall", "Props", "rbProps")))
        {
            ShowTrackingShader();
        }
        else
        {
            ShowBaseShader();
        }

        if (trackT <= 0)
        {
            isTracked = false;
            ShowBaseShader();
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
        var gb = Enum.GetValues(typeof(GroupBehaviour));
        GroupBehaviour = (GroupBehaviour) gb.GetValue(Random.Range(0, gb.Length));

        var cl = Enum.GetValues(typeof(Clothing));
        Clothing = (Clothing) cl.GetValue(Random.Range(0, gb.Length));
    }

    public void Animate()
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
    }

    private void ShowBaseShader()
    {
        foreach (var rend in rends)
            for (var i = 0; i < rend.materials.Length; i++)
            {
                var mat = rend.materials[i];
                mat.shader = baseShaders[i];
            }
    }

    private void ShowTrackingShader()
    {
        foreach (var rend in rends)
        foreach (var mat in rend.materials)
            mat.shader = _trackShader;
    }


    public void DisableMe()
    {
        Fsm.Disable.ResetT();
        Fsm.ChangeState(Fsm.Disable);
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
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