using System;
using System.Collections;
using System.Collections.Generic;
using Environment;
using UnityEngine;

public class Lightswitch : MonoBehaviour
{
	[SerializeField] private RoomObjective  thisRoom;
	private void OnTriggerEnter(Collider other)
	{
		var player = other.GetComponent<FirstPersonController>();
		player.nearestSwitch = this;
	}

	private void OnTriggerExit(Collider other)
	{
		var player = other.GetComponent<FirstPersonController>();
		player.nearestSwitch = null;
	}

	public void TurnOff()
	{
		thisRoom.SetLights(false);
	}
}
