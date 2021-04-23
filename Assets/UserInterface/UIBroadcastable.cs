using System;
using UnityEngine;

namespace DefaultNamespace
{
    public abstract class UIBroadcaster : MonoBehaviour
    {
        public event Action<string> OnBroadcast;

        protected void Setup()
        {
            var ui = FindObjectOfType<UserInterface>();
            OnBroadcast += ui.AddMessageToQueue;
        }

        public void Broadcast(string message)
        {
            OnBroadcast?.Invoke(message);
        }
    }
}