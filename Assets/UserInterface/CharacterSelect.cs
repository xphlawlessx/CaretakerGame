using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _charLabel;
    private string _charName = "";
    [SerializeField] private Button _startButton;

    private void Update()
    {
        _startButton.interactable = _charName.Length > 0;
        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Selectable Character")))
            {
                _charName = hit.collider.name;
                _charLabel.text = _charName;
            }
            else if (EventSystem.current.currentSelectedGameObject == null)
            {
                _charName = "";
                _charLabel.text = "";
            }
        }
    }

    public void OnClickStart()
    {
        PlayerPrefs.SetString("Character", _charName);
        SceneManager.LoadScene("Level");
    }
}