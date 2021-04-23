using UnityEngine;

public class SegwayUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _indicators;

    public void SetSprintUI(int chargeLeft)
    {
        for (var i = 0; i < _indicators.Length; i++) _indicators[i].SetActive(i + 1 <= chargeLeft);
    }
}