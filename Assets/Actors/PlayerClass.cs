using System;
using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    private readonly float sprintTMax = 2f;
    private readonly int staminaMax = 5;
    private bool isSprint;
    private float regenT = 2f;
    private float sprintT = 2f;
    private int stamina = 5;
    public CaretakerClass SelectedClass;
    private Transform _torch;
    private readonly float regenTMax = 2f;
    private bool isRegen;
    private float baseSpeed;
    private FirstPersonController _fpc;
    private void Awake()
    {
        var charname = PlayerPrefs.GetString("Character");
        Debug.Log(charname);
        _fpc = GetComponent<FirstPersonController>();
        if (charname == "Segway")
        {
            SelectedClass = CaretakerClass.Segway;
            var ui = FindObjectOfType<SegwayUI>();
            OnSprint += ui.SetSprintUI;
        }
        else
            FindObjectOfType<SegwayUI>().GetComponentInChildren<Canvas>().enabled = false;
        if (charname == "Assistant")
        {
            SelectedClass = CaretakerClass.Assistant;
        }
        else
        {
            FindObjectOfType<UserInterface>().GetComponentInChildren<Canvas>().enabled = false;
        }

        if (charname == "Tech")
        {
            SelectedClass = CaretakerClass.Tech;
        }
        else
        {
            GameObject.Find("Tech Camera").SetActive(false);
            GameObject.Find("Tech Canvas").SetActive(false);
        }

        if (charname == "Detective")
        {
            SelectedClass = CaretakerClass.Detective;
        }

        
        _torch = _fpc.torch;
        baseSpeed = _fpc.speed;
    }
    private event Action<int> OnSprint;
    private void Update()
    {

        switch (SelectedClass)
        {
            case CaretakerClass.Basic:
                break;
            case CaretakerClass.Segway:
                if (Input.GetKey(KeyCode.Space) && stamina > 0)
                {
                    sprintT -= Time.deltaTime;
                    _fpc.speed = baseSpeed * 4f;
                    if (sprintT < 0f)
                        SpendSprintResource();

                    else
                        isSprint = false;
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
                if (Input.GetMouseButtonDown(0)) Shoot();
                if (Input.GetKeyDown(KeyCode.Space)) RadarPing();
                break;
            case CaretakerClass.Assistant:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    
        

    }

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

        public enum CaretakerClass
        {
            Basic,Segway,Detective,Tech,Assistant
        }
}