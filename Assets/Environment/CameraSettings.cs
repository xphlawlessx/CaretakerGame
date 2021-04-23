using Cinemachine;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _startCam;

    private void Start()
    {
        GetComponentInChildren<CinemachineVirtualCamera>().Follow = FindObjectOfType<FirstPersonController>().transform;
        ;
        _startCam.Priority = 0;
    }
}