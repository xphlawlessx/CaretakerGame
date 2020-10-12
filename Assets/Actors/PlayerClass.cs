using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerClass : MonoBehaviour
{
    public enum CaretakerClass
    {
        Basic,
        Segway,
        Detective,
        Tech,
        Assistant
    }

    public CaretakerClass SelectedClass;
    private readonly float regenTMax = 2f;
    private readonly float shootTMax = 2f;
    private readonly float sprintTMax = 2f;
    private readonly int staminaMax = 5;
    private FirstPersonController _fpc;
    private Transform _torch;
    private float baseSpeed;
    private bool canShoot;
    private bool isRegen;
    private bool isSprint;
    private LevelManager lm;
    private float regenT = 2f;
    private float shootT = 2f;
    private float sprintT = 2f;
    private int stamina = 5;

    private UserInterface _assistantUi;
    private CinemachineVirtualCamera[] _cctvCamsNW;
    private CinemachineVirtualCamera[] _cctvCamsNE;
    private CinemachineVirtualCamera[] _cctvCamsSW;
    private CinemachineVirtualCamera[] _cctvCamsSE;
    private CinemachineVirtualCamera _mainCam;
    private int camIndex = -1;

    private void Awake()
    {
        _assistantUi = FindObjectOfType<UserInterface>();
        var charname = PlayerPrefs.GetString("Character", "");
        if (charname == "") SceneManager.LoadScene("Caretaker Select");
        _fpc = GetComponent<FirstPersonController>();
        lm = FindObjectOfType<LevelManager>();
        if (charname == "Segway")
        {
            SelectedClass = CaretakerClass.Segway;
            var ui = FindObjectOfType<SegwayUI>();
            OnSprint += ui.SetSprintUI;
        }
        else
        {
            FindObjectOfType<SegwayUI>().GetComponentInChildren<Canvas>().enabled = false;
        }

        if (charname == "Assistant")
        {
            SelectedClass = CaretakerClass.Assistant;
            _cctvCamsNW = GameObject.Find("NorthWestCams").GetComponentsInChildren<CinemachineVirtualCamera>();
            _mainCam = FindObjectOfType<CameraSettings>().GetComponentInChildren<CinemachineVirtualCamera>();
        }
        else
            _assistantUi.GetComponentInChildren<Canvas>().enabled = false;

        if (charname == "Tech")
        {
            SelectedClass = CaretakerClass.Tech;
        }
        else
        {
            GameObject.Find("Tech Camera").SetActive(false);
            GameObject.Find("Tech Canvas").SetActive(false);
        }

        if (charname == "Detective") SelectedClass = CaretakerClass.Detective;


        _torch = _fpc.torch;
        baseSpeed = _fpc.speed;
    }

    private void Update()
    {
        switch (SelectedClass)
        {
            case CaretakerClass.Basic:
                break;
            case CaretakerClass.Segway:
                if (Input.GetButton("CaretakerSpecificAction")&& stamina > 0)
                {
                    sprintT -= Time.deltaTime;
                    _fpc.speed = baseSpeed * 4f;
                    if (sprintT < 0f)
                    {
                        SpendSprintResource();
                        lm.AddDamageCashValue(100);
                    }
                    else
                    {
                        isSprint = false;
                    }
                }
                else
                {
                    _fpc.speed = baseSpeed;
                    sprintT = sprintTMax;
                    regenT -= Time.deltaTime;
                    if (regenT < 0)
                    {
                        regenT = regenTMax;
                        RegenSprintCharge();
                    }
                }

                break;
            case CaretakerClass.Detective:
                break;
            case CaretakerClass.Tech:
                if (!canShoot)
                {
                    shootT -= Time.deltaTime;
                    canShoot = shootT <= 0;
                }

                if (Input.GetButtonDown("CaretakerSpecificAction"))
                {
                    if (canShoot)
                    {
                        Debug.Log("shoot");
                        Shoot();
                    }
                    else
                    {
                        Debug.Log("ping");
                        RadarPing();
                    }
                }

                break;
            case CaretakerClass.Assistant:
                if (Input.GetButtonDown("CaretakerSpecificAction"))
                {
                    ActivateCCTV();
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    ResetCam();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ActivateCCTV()
    {
        var message =_assistantUi.Message;
        if(message.Length<1) return;
        if (message.ToLower().Contains("northwest"))
        {
            _mainCam.Priority = 0;
            if (camIndex >= 0 )
            {
                _cctvCamsNW[camIndex].Priority =0;
            }
            camIndex++;
            if (camIndex < _cctvCamsNW.Length)
            {
                _cctvCamsNW[camIndex].Priority =1;
            }
           if(camIndex>_cctvCamsNW.Length-1)
            {
                ResetCam();
            }
        }
    }

    private void ResetCam()
    {
        camIndex = -1;
        _mainCam.Priority = 1;
    }
    private event Action<int> OnSprint;

    public void RadarPing()
    {
        var hits = Physics.OverlapSphere(transform.position, 150f);
        foreach (var e in hits)
        {
            var enemy = e.GetComponent<Kid>();
            if (!enemy) continue;
            enemy.ShowOnMiniMap(5f);
        }
    }


    public void Shoot()
    {
        canShoot = false;
        shootT = shootTMax;
        RaycastHit hit;
        var pos = _torch.position;
        if (Physics.Raycast(pos, _torch.forward,
            out hit, 500f, LayerMask.GetMask("Wall", "Enemy")))
            if (hit.collider.CompareTag("Enemy"))
            {
                var enemy = hit.collider.GetComponent<Kid>();
                enemy.ShowOnMiniMap(15f);
            }
    }

    public void RegenSprintCharge()
    {
        if (stamina < staminaMax)
        {
            stamina += 1;
            OnSprint.Invoke(stamina);
        }
    }

    public void SpendSprintResource()
    {
        sprintT = sprintTMax;
        stamina -= 1;
        OnSprint.Invoke(stamina);
    }
}