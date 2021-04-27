using System;
using UnityEngine;

namespace UserInterface
{
    public abstract class UIBroadcaster : MonoBehaviour
    {
        public event Action<string> OnBroadcast;

        protected void Setup()
        {
            var dialogBox = FindObjectOfType<DialogBox>();
            OnBroadcast += dialogBox.AddMessageToQueue;
        }

        protected void Broadcast(string message)
        {
            OnBroadcast?.Invoke(message);
        }
    }
}