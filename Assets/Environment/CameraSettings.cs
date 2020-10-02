using Cinemachine;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private LayerMask enviro;
    private float _baseCamDist;
    private Transform _cam;
    private int _camIndex;
    private Transform _player;
    private CinemachineVirtualCamera[] _vCams;

    private void Start()
    {
        _vCams = GetComponentsInChildren<CinemachineVirtualCamera>();
        _player = FindObjectOfType<FirstPersonController>().transform;
        _baseCamDist = Vector3.Distance(_vCams[0].transform.position, _player.transform.position);
        _cam = Camera.main.transform;
    }

    private void Update()
    {
        var pPos = _player.position;
        var camPos = pPos + (_cam.position - pPos).normalized * _baseCamDist;
        if (Physics.Linecast(camPos, pPos , enviro))
            ActivateTightCam();
        else
            ActivateMainCam();
    }

    private void ActivateTightCam()
    {
        //print("Tight");
        if (_camIndex == 0)
        {
            _vCams[0].Priority = 0;
            _camIndex = 1;
            _vCams[1].Priority = 9;
        }
    }

    private void ActivateMainCam()
    {
        //print("Main");
        if (_camIndex == 1)
        {
            _vCams[1].Priority = 0;
            _camIndex = 0;
            _vCams[0].Priority = 9;
        }
    }
}