using TMPro;
using UnityEngine;

public class ScoreLevel : MonoBehaviour
{
    private void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text =
            $"Time Remaining: {PlayerPrefs.GetFloat("Time")},\n Kids Escaped: {PlayerPrefs.GetInt("Escaped")},\n  Kids Identified: {PlayerPrefs.GetInt("Identified")},\n Damage Done: £{PlayerPrefs.GetInt("Damage")}";
    }
}