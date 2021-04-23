using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _queueText;
    private readonly float _dialogueTMax = 4f;
    private readonly Queue<string> _messages = new Queue<string>();
    private GameObject _dialogueParent;
    private float _dialogueT;
    private TextMeshProUGUI _dialogueText;
    private bool _isShowDialogue;

    public string Message => _dialogueText.text;

    private void Start()
    {
        _dialogueParent = GameObject.Find("DialoguePanel");
        _dialogueText = _dialogueParent.GetComponentInChildren<TextMeshProUGUI>();
        HideDialogue();
    }

    private void Update()
    {
        if (_messages.Count == 0 && !_isShowDialogue) return;
        if (_dialogueT > 0)
        {
            _dialogueT -= Time.deltaTime;
            return;
        }

        if (_messages.Count == 0 && _isShowDialogue)
        {
            HideDialogue();
            return;
        }

        SetMessageFromQueue();
    }

    public void HideDialogue()
    {
        _isShowDialogue = false;
        _dialogueT = 0;
        _dialogueText.text = "";
        _dialogueParent.SetActive(false);
    }

    private void SetMessageFromQueue()
    {
        _dialogueParent.SetActive(true);
        _dialogueText.text = _messages.Dequeue();
        _dialogueT = _dialogueTMax;
        _isShowDialogue = true;
        _queueText.text = _messages.Count.ToString();
    }

    public void AddMessageToQueue(string message)
    {
        if (_messages.Contains(message)) return;
        if (!_isShowDialogue && _messages.Count == 0)
        {
            _messages.Enqueue(message);
            SetMessageFromQueue();
        }
        else
        {
            _messages.Enqueue(message);
        }

        _queueText.text = _messages.Count.ToString();
    }
}