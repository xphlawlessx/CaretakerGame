using UnityEngine;
using UnityEngine.AI;

public class FirstPersonController : MonoBehaviour
{
    private const float RotSpeed = 8f;
    private const float CamOffset = 20f;

    private static readonly int IsWalk = Animator.StringToHash("IsWalk");

    //bool isStartBlend = true;
    //CinemachinePOV _vCam;
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private Animator _anim;
    private Camera _cam;
    private float _coneR;
    private Vector3 _dir;
    private bool _isRotating;
    private NavMeshAgent _nav;
    public float speed = 5f;
    [SerializeField] public Transform torch;
    public Lightswitch nearestSwitch { get; set; }

    private void Start()
    {
        _coneR = Mathf.Cos(35 * Mathf.Deg2Rad);
        _cam = Camera.main;
        _nav = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        // _vCam = GameObject.Find("MainCam").GetComponent<CinemachineVirtualCamera>()
        //     .GetCinemachineComponent<CinemachinePOV>();
        // _vCam.m_HorizontalAxis.m_InputAxisName = "";
        // _vCam.m_VerticalAxis.m_InputAxisName = "";
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log(CinemachineCore.Instance.IsLive(_vCam.VirtualCamera));
        // if (isStartBlend)
        // {
        //     if(CinemachineCore.Instance.IsLive(_vCam.VirtualCamera))
        //         return;
        //     else
        //     {
        //         isStartBlend = false;
        //         Debug.Log("not blending");
        //         _vCam.m_HorizontalAxis.m_InputAxisName = "Mouse X";
        //         _vCam.m_VerticalAxis.m_InputAxisName = "Mouse Y";
        //     }
        // }
        _dir = GetDirInput();
        var isMove = _dir != Vector3.zero;
        _anim.SetBool(IsWalk, isMove);
        if (isMove)
        {
            var movement = _cam.transform.TransformDirection(_dir);
            _nav.Move(movement * speed * Time.deltaTime);
        }

        if (nearestSwitch && Input.GetButton("Interact")) nearestSwitch.TurnOff();
        var ray = _cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        Quaternion direction;
        direction = Quaternion.LookRotation(ray.direction.normalized * CamOffset);
        var rotAngle = GetRotationAngleThisFrame(transform.rotation, direction);
        _anim.SetFloat(Horizontal, rotAngle);
        _anim.SetFloat(Vertical, _dir.z);

        transform.rotation = Quaternion.Lerp(transform.rotation,
            direction, RotSpeed * Time.deltaTime);

        var hits = Physics.OverlapSphere(transform.position, 15f);
        foreach (var e in hits)
        {
            var enemy = e.GetComponent<Kid>();
            if (!enemy) continue;
            var ePos = e.transform.position;
            var pos = torch.position;
            var dir = (ePos - pos).normalized;
            if (!(Vector3.Dot(torch.forward, dir) >= _coneR)) continue;
            enemy.IsInLight = !Physics.Linecast(pos, ePos,
                LayerMask.GetMask("Wall"));
        }
    }


    private float GetRotationAngleThisFrame(Quaternion fromRotation, Quaternion toRotation)
    {
        // get a "forward vector" for each rotation
        var forwardA = fromRotation * Vector3.forward;
        var forwardB = toRotation * Vector3.forward;


// get a numeric angle for each vector, on the X-Z plane (relative to world forward)
        var angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
        var angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;


// get the signed difference in these angles
        return Mathf.DeltaAngle(angleA, angleB);
    }

    private Vector3 GetDirInput()
    {
        return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }
}